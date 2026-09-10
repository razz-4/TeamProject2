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
    [SerializeField] int minItemsPerBuyer = 1;
    [SerializeField] int maxItemsPerBuyer = 6;
    [SerializeField] int minAmountPerItem = 10;
    [SerializeField] int maxAmountPerItem = 100;

    private VisualElement root;
    private VisualElement deliveryUIPanel;
    private VisualElement itemDisplayPanel;
    private ScrollView buyerScrollView;
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

        sellBtn.RegisterCallback<ClickEvent>(OnSellPressed);
        closeBtn.RegisterCallback<ClickEvent>(OnClosePressed);

        EventManager.Instance.refreshBuyerList += HandleRefreshBuyers;
        HandleRefreshBuyers();

    }

    private void HandleRefreshBuyers()
    {
        RefreshBuyers(GenerateBuyers());
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

        for (int i = 0; i < buyerCount; i++)
        {
            bool isIllegal = UnityEngine.Random.value < GameManager.Instance.illegalRate;

            var requestedItems = new List<ItemSlot>();
            int itemCount = UnityEngine.Random.Range(minItemsPerBuyer, maxItemsPerBuyer + 1);

            for (int j = 0; j < itemCount; j++)
            {
                bool wantsDrug = UnityEngine.Random.value < GameManager.Instance.drugRate;

                Item chosenItem = wantsDrug
                    ? ItemManager.Instance.drugs[UnityEngine.Random.Range(0, ItemManager.Instance.drugs.Length)]
                    : ItemManager.Instance.ingredients[UnityEngine.Random.Range(0, ItemManager.Instance.ingredients.Length)];

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
                buyerName = GenerateBuyerName(), //isIllegal
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

        EventManager.Instance.InvokeInventoryChanged();

        currentBuyers.Remove(selectedBuyer);
        RefreshBuyers(currentBuyers);
    }

    private void OnClosePressed(ClickEvent evt)
    {
        deliveryUIPanel.style.display = DisplayStyle.None;
        SoundManager.PlaySound(SoundType.UI);
    }


    private string GenerateBuyerName() //bool isIllegal
    {
        BuyerName[] buyerNameAssets = Utils.GetAllBuyerNameInstances();


        BuyerName randomAsset = buyerNameAssets[UnityEngine.Random.Range(0, buyerNameAssets.Length)];
        string randomName = randomAsset.GetRandomName();

        string temp = randomName;

        //if (isIllegal)
        //{
        //    temp = "Illegal " + randomName;
        //}

        return temp;
    }
}
