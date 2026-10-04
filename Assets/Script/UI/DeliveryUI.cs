using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField] UIDocument deliveryUI;
    [SerializeField] VisualTreeAsset buyerSlotTemplate;
    [SerializeField] VisualTreeAsset itemDisplayTemplate;

    [SerializeField] int buyerCount = 5;
    [SerializeField] int amountNeeded = 5;
    [SerializeField] int minItemsPerBuyer = 1;
    [SerializeField] int maxItemsPerBuyer = 3;
    [SerializeField] int minAmountPerItem = 10;
    [SerializeField] int maxAmountPerItem = 50;
    [SerializeField] private float refreshTime = 30f;

    private VisualElement root;
    private VisualElement deliveryUIPanel;
    private VisualElement itemDisplayPanel;
    private ScrollView buyerScrollView;
    private Coroutine _refreshRoutine;
    private Label buyerNameLabel;
    private Label totalValueLabel;


    private Buyer selectedBuyer;
    private List<Buyer> currentBuyers = new List<Buyer>();


    void Start()
    {
        root = deliveryUI.rootVisualElement;

        var sellBtn = root.Q<VisualElement>("SellButton");
        var closeBtn = root.Q<VisualElement>("CloseButton");

        deliveryUIPanel = root.Q<VisualElement>("DeliveryUIPanel");
        itemDisplayPanel = root.Q<VisualElement>("ItemsDisplayGrid");
        buyerScrollView = root.Q<ScrollView>("BuyerScrollView");
        buyerNameLabel = root.Q<Label>("NameLabel");
        totalValueLabel = root.Q<Label>("ValueLabel");

        deliveryUIPanel.AddToClassList("SettingUp");

        sellBtn.RegisterCallback<ClickEvent>(OnSellPressed);
        closeBtn.RegisterCallback<ClickEvent>(OnClosePressed);

        EventManager.Instance.refreshBuyerList += HandleRefreshBuyers;
        HandleRefreshBuyers();

        _refreshRoutine = StartCoroutine(AutoRefreshRoutine());
    }

    private void HandleRefreshBuyers()
    {
        RefreshBuyers(GenerateBuyers());
    }

    private System.Collections.IEnumerator AutoRefreshRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshTime);
            HandleRefreshBuyers();
        }
    }

    private void OnDestroy()
    {
        if (_refreshRoutine != null)
            StopCoroutine(_refreshRoutine);

        EventManager.Instance.refreshBuyerList -= HandleRefreshBuyers;
    }

    private void RefreshBuyers(List<Buyer> buyers)
    {
        currentBuyers = buyers;
        selectedBuyer = null;
        buyerNameLabel.text = "";

        itemDisplayPanel.Clear();
        buyerScrollView.Clear();

        foreach (Buyer buyer in currentBuyers)
        {
            VisualElement slotInstance = buyerSlotTemplate.Instantiate();
            var slotUI = new BuyerSlotUI(slotInstance);
            slotUI.SetBuyer(buyer);
            slotUI.OnSlotClicked += SelectBuyer;

            buyerScrollView.Add(slotInstance);
        }
    }

    private void SelectBuyer(Buyer buyer)
    {
        selectedBuyer = buyer;
        buyerNameLabel.text = buyer.buyerName;
        totalValueLabel.text = buyer.totalValue.ToString();
        itemDisplayPanel.Clear();

        foreach (ItemSlot request in buyer.requestedItem)
        {
            VisualElement slotInstance = itemDisplayTemplate.Instantiate();
            var slotUI = new ItemDisplaySlotUI(slotInstance);
            slotUI.SetItem(request);

            itemDisplayPanel.Add(slotInstance);
        }
    }

    private List<Buyer> GenerateBuyers()
    {
        var buyers = new List<Buyer>();

        //Concat combines Drugs and Ingredients into one single list so this list contains both drugs and ingredients.
        //Cast treats the drugs and ingredients as an Item allowing them to be in one list.
        //Where make sure about 10 or more amount exist to make it to the list
        List<Item> eligibleItems = ItemManager.Instance.drugs.Cast<Item>().Concat(ItemManager.Instance.ingredients.Cast<Item>())
        .Where(item => item.quantity > amountNeeded).ToList();

        for (int i = 0; i < buyerCount; i++)
        {
            bool isIllegal = UnityEngine.Random.value < GameManager.Instance.illegalRate;

            var requestedItems = new List<ItemSlot>();
            int itemCount = UnityEngine.Random.Range(minItemsPerBuyer, maxItemsPerBuyer + 1);

            // Copy so removals here don't affect other buyers or the master list.
            List<Item> availableForThisBuyer = new List<Item>(eligibleItems);

            for (int j = 0; j < itemCount; j++)
            {
                if (availableForThisBuyer.Count == 0)
                    break; // no more unique items left to give this buyer

                int index = UnityEngine.Random.Range(0, availableForThisBuyer.Count);
                Item chosenItem = availableForThisBuyer[index];
                availableForThisBuyer.RemoveAt(index); // prevents picking it again for this buyer

                int amount = UnityEngine.Random.Range(minAmountPerItem, maxAmountPerItem + 1);

                requestedItems.Add(new ItemSlot { item = chosenItem, amount = amount });
            }

            int payout = requestedItems.Sum(r => r.item.value * r.amount);
            if (isIllegal)
            {
                payout = Mathf.RoundToInt(payout * GameManager.Instance.illegalMultiplier);
            }

            buyers.Add(new Buyer
            {
                buyerName = GenerateBuyerName(isIllegal),
                isIllegal = isIllegal,
                requestedItem = requestedItems,
                totalValue = payout
            });
        }

        return buyers;
    }
    private void OnSellPressed(ClickEvent evt)
    {
        if (selectedBuyer == null) return;

        foreach (ItemSlot request in selectedBuyer.requestedItem)
        {
            Item stock = ItemManager.Instance.ingredients
                .Cast<Item>()
                .Concat(ItemManager.Instance.drugs.Cast<Item>())
                .FirstOrDefault(i => i == request.item);

            if (stock == null || stock.quantity < request.amount)
            {
                Debug.Log($"Not enough {request.item.itemName} to fulfill {selectedBuyer.buyerName}'s order");
                return;
            }
        }

        foreach (ItemSlot request in selectedBuyer.requestedItem)
        {
            request.item.quantity -= request.amount;
        }

        SoundManager.PlaySound(SoundType.SOLD);
        GameManager.Instance.AddGold(selectedBuyer.totalValue);

        //Tutorial Part
        if (TutorialManager.Instance.dialogPart == 3) { TutorialManager.Instance.GoNextDialog(); }

        EventManager.Instance.InvokeInventoryChanged();

        currentBuyers.Remove(selectedBuyer);
        RefreshBuyers(currentBuyers);
    }

    private void OnClosePressed(ClickEvent evt)
    {
        deliveryUIPanel.AddToClassList("SettingUp");
        SoundManager.PlaySound(SoundType.UI);
    }


    private string GenerateBuyerName(bool isIllegal) //bool isIllegal
    {
        BuyerName[] buyerNameAssets = Utils.GetAllBuyerNameInstances();
        BuyerName randomAsset = buyerNameAssets[UnityEngine.Random.Range(0, buyerNameAssets.Length)];
        string randomName = randomAsset.GetRandomName();


        return isIllegal ? "Illegal " + randomName : randomName;
    }
}
