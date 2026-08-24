using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public event Action OnInventoryChanged;
    public event Action generateItems;
    public event Action refreshBuyerList;

    public void InvokeInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void InvokeGenerateItems()
    {
        generateItems?.Invoke();
    }

    public void InvokeRefreshBuyerList()
    {
        refreshBuyerList?.Invoke();
    }

}