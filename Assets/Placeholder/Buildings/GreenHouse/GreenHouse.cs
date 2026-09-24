using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BuildingHighlight))]
[RequireComponent(typeof(BuildingTimerController))]
[RequireComponent(typeof(BuildingPopUpSpawner))]
public class GreenHouse : BuildingClass
{
    public int maxToggle;

    [Header("Input System")]
    private MobileAction input;

    [Header("Green House Storage")]
    [SerializeField] private int maxGreenHouseStorage = 50;
    private int currentGreenHouseStorage;

    [HideInInspector] public Ingredient currentIngredient;
    [HideInInspector] private Ingredient lastIngredient;

    // Component References
    private BuildingHighlight buildingHighlight;
    private BuildingTimerController timerController;
    private BuildingPopUpSpawner popUpSpawner;

    // Save index positions instead of object references
    public List<int> selectedIndices = new List<int>();
    private List<Ingredient> currentGrowingIngredients = new List<Ingredient>();

    private void Awake()
    {
        input = new MobileAction();

        // Cache all attached modules
        buildingHighlight = GetComponent<BuildingHighlight>();
        timerController = GetComponent<BuildingTimerController>();
        popUpSpawner = GetComponent<BuildingPopUpSpawner>();

        buildingType = "GreenHouse";
        placable = true;
        level = 1;
        isLock = false;
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
        PlantIngredient(currentIngredient);
        SetLayerToCollectable();
        buildingHighlight.EnableHighlight();
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

        CollectIngredient();
    }
    #endregion

    public void SetIngredient(Ingredient ingredient)
    {
        if (lastIngredient == null)
        {
            lastIngredient = ingredient;
        }
        else
        {
            lastIngredient = currentIngredient;
        }
        currentIngredient = ingredient;
    }

    public void PlantIngredient(Ingredient ingredient)
    {
        currentGreenHouseStorage += ingredient.currentPerSecond;
        if (currentGreenHouseStorage >= maxGreenHouseStorage)
        {
            currentGreenHouseStorage = maxGreenHouseStorage;
            timerController.StopProductionTimer();
        }

        Debug.Log($"Current green house storage: {currentGreenHouseStorage}");
    }

    public void SetLayerToCollectable()
    {
        gameObject.layer = LayerMask.NameToLayer("Collectable");
    }

    public void CollectIngredient()
    {
        if (currentGreenHouseStorage <= 0) return;

        int collectableLayer = LayerMask.GetMask("Collectable");
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, collectableLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.transform != transform) return;

            bool wasFull = currentGreenHouseStorage >= maxGreenHouseStorage;

            foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
            {
                if (ingredient == currentIngredient)
                {
                    ingredient.quantity += currentGreenHouseStorage;
                    popUpSpawner.SpawnPopUp(currentIngredient.icon, currentGreenHouseStorage);

                    Debug.Log($"Added {currentGreenHouseStorage} {ingredient.name}");
                    Debug.Log($"Current quantity of {ingredient.name}: {ingredient.quantity}");
                    SoundManager.PlaySound(SoundType.COLLECT, 1);

                    currentGreenHouseStorage = 0;
                }
            }

            gameObject.layer = LayerMask.NameToLayer("Interactable");
            buildingHighlight.DisableHighlight();

            if (wasFull)
            {
                StartTimer();
            }

            //Tutorial part
            if (TutorialManager.Instance.dialogPart == 2) { TutorialManager.Instance.GoNextDialog(); }
        }
    }

    public void CollectLastIngredient()
    {
        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == lastIngredient && currentGreenHouseStorage != 0)
            {
                ingredient.quantity += currentGreenHouseStorage;
                popUpSpawner.SpawnPopUp(lastIngredient.icon, currentGreenHouseStorage);
                lastIngredient.produce = false;

                Debug.Log($"Added {currentGreenHouseStorage} {ingredient.name}");
                Debug.Log($"Current quantity of {ingredient.name}: {ingredient.quantity}");

                currentGreenHouseStorage = 0;
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Interactable");
        buildingHighlight.DisableHighlight();
    }

    public void StartTimer()
    {
        timerController.StartProductionTimer();
    }

    public LoadingTimer GetTimer()
    {
        return timerController.GetTimer();
    }


    public override void LevelUp()
    {
        level++;

        Debug.Log("GreenHouse Level Up! New Level: " + level);

        GreenHouseUI greenHouseUI = FindAnyObjectByType<GreenHouseUI>(FindObjectsInactive.Include);

        if (greenHouseUI != null)
        {
            greenHouseUI.RefreshAllLockStates();
        }
        else
        {
            Debug.LogWarning("Could not find GreenHouseUI!");
        }
    }


}