using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BuyerSlotUI
{
    private VisualElement root;
    public Buyer storedBuyer { get; private set;  }


    private Label buyerName;
    private Label value;

    public event Action<Buyer> OnSlotClicked;

    public BuyerSlotUI(VisualElement root)
    {
        this.root = root;

        buyerName = root.Q<Label>("BuyerName");
        value = root.Q<Label>("Value");

        root.RegisterCallback<ClickEvent>(evt => OnSlotClicked?.Invoke(storedBuyer));
    }

    public void SetBuyer(Buyer buyer)
    {
        storedBuyer = buyer;
        buyerName.text = buyer.buyerName;
        value.text = buyer.totalValue.ToString();
    }
}
