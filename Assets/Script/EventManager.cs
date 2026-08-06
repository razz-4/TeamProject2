using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public event Action OnInventoryChanged;

    public void InvokeInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

}
