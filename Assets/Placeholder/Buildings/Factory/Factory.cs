using DevelopersHub.ClashOfWhatecer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BuildingHighlight))]
[RequireComponent(typeof(BuildingTimerController))]
[RequireComponent(typeof(BuildingPopUpSpawner))]
public class Factory : BuildingClass
{
    [Header("Input System")]
    private MobileAction input;

    [Header("Factory Storage")]
    [SerializeField] private int maxFactoryStorage = 50;
    private int currentFactoryStorage;

    [HideInInspector] public Drug currentDrug;

    // Component References
    private BuildingHighlight buildingHighlight;
    private BuildingTimerController timerController;
    private BuildingPopUpSpawner popUpSpawner;

    // Selected drug index tracking
    public List<int> selectedDrugs = new List<int>();

    private void Awake()
    {
        input = new MobileAction();

        // Cache attached modules
        buildingHighlight = GetComponent<BuildingHighlight>();
        timerController = GetComponent<BuildingTimerController>();
        popUpSpawner = GetComponent<BuildingPopUpSpawner>();

        buildingType = "Factory";
        placable = true;
        level = 1;
        isLock = false;
        timerController.StopProductionTimer();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Mobile.Interact.performed += OnInteract;

        if (timerController != null && timerController.OnTimerFinished != null)
        {
            timerController.OnTimerFinished.AddListener(OnTimerFinished);
        }
    }

    private void OnDisable()
    {
        input.Mobile.Interact.performed -= OnInteract;

        if (timerController != null && timerController.OnTimerFinished != null)
        {
            timerController.OnTimerFinished.RemoveListener(OnTimerFinished);
        }

        input.Disable();
    }


    private void OnTimerFinished()
    {
        // Try synthesizing when the timer finishes a cycle
        if (currentDrug != null)
        {
            if (Synthesize(currentDrug))
            {
                SetLayerToCollectable();
                buildingHighlight.EnableHighlight();
            }
            else
            {
                // Not enough resources, stop processing
                timerController.StopProductionTimer();
            }
        }
    }

    #region Interaction Function
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            {
                return;
            }
        }

        CollectDrug();
    }
    #endregion

    public void SelectDrugToProduce(Drug drug, int index)
    {
        currentDrug = drug;

        if (!selectedDrugs.Contains(index))
        {
            selectedDrugs.Add(index);
        }

        // Check if we can afford the initial run before starting
        if (CanSynthesize(drug))
        {
            StartProduction();
            UIBuildingManager.Instance.HideUI();
        }
        else
        {
            Debug.Log($"Cannot start producing {drug.name}: Missing required ingredients.");
        }
    }

    public void CancelDrug(Drug drug, int index)
    {
        if (selectedDrugs.Contains(index))
        {
            selectedDrugs.Remove(index);
        }

        if (currentDrug == drug)
        {
            currentDrug = null;
            timerController.StopProductionTimer();
        }
    }

    #region Synthesis Logic
    public bool CanSynthesize(Drug drug)
    {
        if (drug == null || drug.recipe == null) return false;

        foreach (IngredientRequirement req in drug.recipe)
        {
            Ingredient stock = ItemManager.Instance.ingredients.FirstOrDefault(i => i.id == req.ingredient.id);
            int needed = req.amount;

            if (stock == null || stock.quantity < needed)
            {
                Debug.Log($"Not enough {req.ingredient.name} to synthesize {drug.name}");
                timerController.StopProductionTimer();
                return false;
            }
        }

        return true;
    }

    public bool Synthesize(Drug drug)
    {
        // Step 1: Check ingredient requirements against ItemManager
        if (!CanSynthesize(drug))
        {
            return false;
        }

        // Step 2: Deduct ingredients from ItemManager inventory
        foreach (IngredientRequirement req in drug.recipe)
        {
            Ingredient stock = ItemManager.Instance.ingredients.First(i => i.id == req.ingredient.id);
            stock.quantity -= req.amount;
        }

        // Step 3: Add produced batch to local storage
        int batchAmount = drug.perSecond;
        currentFactoryStorage += batchAmount;

        if (currentFactoryStorage >= maxFactoryStorage)
        {
            currentFactoryStorage = maxFactoryStorage;
            timerController.StopProductionTimer();
        }

        Debug.Log($"Synthesized {drug.name}. Current Factory Storage: {currentFactoryStorage}/{maxFactoryStorage}");
        return true;
    }
    #endregion

    #region Collection Logic
    public void CollectDrug()
    {
        if (currentFactoryStorage <= 0 || currentDrug == null) return;

        int collectableLayer = LayerMask.GetMask("Collectable");
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, collectableLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.transform != transform) return;

            bool wasFull = currentFactoryStorage >= maxFactoryStorage;

            // Update drug inventory
            currentDrug.quantity += currentFactoryStorage;
            popUpSpawner.SpawnPopUp(currentDrug.icon, currentFactoryStorage);

            //PLay Sound
            SoundManager.PlaySound(SoundType.CHEMICAL, 1);

            Debug.Log($"Collected {currentFactoryStorage} {currentDrug.name}. New total: {currentDrug.quantity}");

            currentFactoryStorage = 0;

            gameObject.layer = LayerMask.NameToLayer("Interactable");
            buildingHighlight.DisableHighlight();

            // Resume production timer if it was paused due to full storage
            if (wasFull && CanSynthesize(currentDrug))
            {
                StartProduction();
            }
        }
    }
    #endregion

    public void SetLayerToCollectable()
    {
        gameObject.layer = LayerMask.NameToLayer("Collectable");
    }

    public void StartProduction()
    {
        timerController.StartProductionTimer();
        Debug.Log("StartTimer");
    }

    public LoadingTimer GetTimer()
    {
        return timerController.GetTimer();
    }

    public override void LevelUp()
    {
        level++;

        BuildingControl bc = GetComponent<BuildingControl>();
        if (bc != null)
            bc.UpdateLevelVisual(level);

        FactoryUI factoryUI = FindAnyObjectByType<FactoryUI>(FindObjectsInactive.Include);
        if (factoryUI != null)
        {
            factoryUI.RefreshAllLockStates();
        }
    }
}