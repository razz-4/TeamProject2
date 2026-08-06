using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StorageSlotUI
{
    private VisualElement root;

    public Item storedItem { get; private set; }

    private Image itemImage;
    private Label itemName;
    private Label amount;

    public StorageSlotUI(VisualElement root)
    {
        this.root = root;

        itemImage = root.Q<Image>("ItemImage");
        itemName = root.Q<Label>("ItemName");
        amount = root.Q<Label>("Amount");
    }

    public void SetItem(Item item, int quantity)
    {
        storedItem = item;

        itemName.text = item.itemName;
        amount.text = quantity.ToString();




        if (item.icon != null)
        {
            itemImage.sprite = item.icon;

            itemImage.style.display = item.icon != null ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void Clear()
    {
        storedItem = null;
        itemName.text = string.Empty;
        amount.text = string.Empty;

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.style.display = DisplayStyle.None;
        }
    }
}