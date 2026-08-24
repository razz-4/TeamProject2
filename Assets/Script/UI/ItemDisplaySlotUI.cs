using UnityEngine.UIElements;

public class ItemDisplaySlotUI
{
    private VisualElement root;

    private Image itemImage;
    private Label itemQuantity;

    public ItemDisplaySlotUI(VisualElement root)
    {
        this.root = root;

        itemImage = root.Q<Image>("ItemImage");
        itemQuantity = root.Q<Label>("ItemQuantity");
    }

    public void SetItem(ItemSlot itemSlot)
    {
        itemQuantity.text = "x" + itemSlot.amount;

        if (itemSlot.item.icon != null)
        {
            itemImage.sprite = itemSlot.item.icon;
        }
    }
}
