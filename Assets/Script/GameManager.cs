using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Properties;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    [SerializeField] int gold = 500;
    public float illegalRate = 0.7f;
    public float drugRate = 0.7f;
    public float illegalMultiplier = 1.5f;

    [CreateProperty]
    public string goldDisplay => $"${gold}";


    private float itemProductionTIme = 1f;
    private float refreshBuyerTime = 600f;
    private float currentItemProductionTIme = 0f;
    private float CurrentRefreshBuyerTime = 0f;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentItemProductionTIme = itemProductionTIme;
        CurrentRefreshBuyerTime = refreshBuyerTime;
    }

    void Update()
    {

        HandleProductionTimer();
        HandleBuyerRefreshListTimer();
        
    }

    private void HandleProductionTimer()
    {
        currentItemProductionTIme -= Time.deltaTime;

        if (currentItemProductionTIme <= 0f)
        {
            EventManager.Instance.InvokeGenerateItems();

            currentItemProductionTIme += itemProductionTIme;
        }

    }

    private void HandleBuyerRefreshListTimer()
    {
        CurrentRefreshBuyerTime -= Time.deltaTime;

        if (CurrentRefreshBuyerTime <= 0f)
        {
            EventManager.Instance.InvokeRefreshBuyerList();

            CurrentRefreshBuyerTime += refreshBuyerTime;
        }

    }
    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        NotifyGoldChanged();
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        NotifyGoldChanged();

    }

    private void NotifyGoldChanged()
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(nameof(goldDisplay)));
    }
}
