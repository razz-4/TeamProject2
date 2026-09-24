using NUnit.Framework;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class StorageUI : MonoBehaviour
{
    [SerializeField] UIDocument storageUI;
    [SerializeField] VisualTreeAsset storageSlotTemplate;
    [SerializeField] private float slotHeight = 40f;


    private VisualElement root;
    private VisualElement storageUIPanel;

    private ListView ingredientListView;
    private ListView drugListView;

    

    void Start()
    {
        root = storageUI.rootVisualElement;

        VisualElement closeBtn = root.Q<VisualElement>("CloseButton");

        storageUIPanel = root.Q<VisualElement>("StorageUiPanel");

        ingredientListView = root.Q<ListView>("IngredientListView");
        drugListView = root.Q<ListView>("DrugListView");

        closeBtn.RegisterCallback<ClickEvent>(CloseStorageUI);

        EventManager.Instance.OnInventoryChanged += RefreshLists;

        storageUIPanel.AddToClassList("MainPanelUp");

        SetupListView(ingredientListView, ItemManager.Instance.ingredients);
        SetupListView(drugListView, ItemManager.Instance.drugs);

        var storageBtn = root.Q<Button>("Storage");
    }

    private void SetupListView<T>(ListView listView, T[] items) where T : Item
    {
        if (listView == null || items == null) return;

        listView.itemsSource = items;
        listView.fixedItemHeight = slotHeight;

        listView.makeItem = () =>
        {
            VisualElement slotInstance = storageSlotTemplate.Instantiate();
            slotInstance.userData = new StorageSlotUI(slotInstance);
            return slotInstance;
        };

        listView.bindItem = (element, index) =>
        {
            var slotUI = element.userData as StorageSlotUI;
            var item = items[index];

            if (slotUI != null && item != null)
            {
                slotUI.SetItem(item, item.quantity);
            }
        };
    }

    private void RefreshLists()
    {
        ingredientListView.RefreshItems();
        drugListView.RefreshItems();
    }

    void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnInventoryChanged -= RefreshLists;
        }
    }


    void CloseStorageUI(ClickEvent evt)
    {
        storageUIPanel.AddToClassList("MainPanelUp");
        SoundManager.PlaySound(SoundType.UI);
    }
}
