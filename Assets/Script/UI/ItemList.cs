using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    [SerializeField] private IngredientDescription ingredientDescription;
    [SerializeField] private ToggleGroup toggleGroup;

    [SerializeField] Toggle[] toggleButtons;

    public int maxToggle;
    int currentToggleActive = 0;

    public GameObject Greenhouse;
    GreenHouse greenHouseScript;

    // Track the currently targeted greenhouse instance
    private GameObject currentGreenhouseObj;

    private void Awake()
    {
        InitializeToggles();
    }

    private void OnEnable()
    {
        // Re-bind group references when the object/panel gets enabled
        RegisterTogglesToGroup();
    }

    private void Start()
    {
        if (toggleButtons != null && toggleButtons.Length > 0 && toggleButtons[0] != null)
        {
            toggleButtons[0].SetIsOnWithoutNotify(true);
        }
    }

    private void InitializeToggles()
    {
        // Pass 'true' to include inactive children in hierarchy
        toggleButtons = GetComponentsInChildren<Toggle>(true);
        RegisterTogglesToGroup();
    }

    private void RegisterTogglesToGroup()
    {
        if (toggleButtons == null || toggleGroup == null) return;

        foreach (Toggle toggle in toggleButtons)
        {
            if (toggle != null)
            {
                toggle.group = toggleGroup;
            }
        }
    }

    private void OnToggleChanged(int index, bool state)
    {
        if (greenHouseScript == null) return;

        Ingredient targetIngredient = ItemManager.Instance.ingredients[index];

        if (state)
        {
            if (currentToggleActive < maxToggle)
            {

                //Ingredient Description
                ingredientDescription.currentGreenHouse = currentGreenhouseObj.GetComponent<GreenHouse>();
                ingredientDescription.ChangeDescription(targetIngredient,greenHouseScript);
                Debug.Log($"[Building: {currentGreenhouseObj.name}] Added index {index}: {targetIngredient.name}");
            }
            else
            {
                // Over limit: forcefully revert toggle UI
                toggleButtons[index].SetIsOnWithoutNotify(false);
                Debug.Log("Toggle limit reached! Max allowed: " + maxToggle);
            }
        }
        else
        {
            if (currentToggleActive > 0)
            {
                Debug.Log($"[Building: {currentGreenhouseObj.name}] Removed index {index}: {targetIngredient.name}");
            }
        }
    }

    public void CheckGreenHouse(GameObject greenHouse, string currentBuildingType)
    {
        if (currentBuildingType != "GreenHouse" || greenHouse == null) return;

        if (currentGreenhouseObj == greenHouse) return;

        currentGreenhouseObj = greenHouse;
        greenHouseScript = greenHouse.GetComponent<GreenHouse>();

        if (toggleButtons == null || toggleButtons.Length == 0)
        {
            toggleButtons = GetComponentsInChildren<Toggle>(true);
        }

        currentToggleActive = greenHouseScript.selectedIndices.Count;

        // Refresh description toggle UI for the newly selected building
        if (ingredientDescription != null && ingredientDescription.currentIngredient != null)
        {
            ingredientDescription.ChangeDescription(ingredientDescription.currentIngredient, greenHouseScript);
        }

        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;

            toggleButtons[i].onValueChanged.RemoveAllListeners();

            bool isSavedInBuilding = greenHouseScript.selectedIndices.Contains(index);
            toggleButtons[i].SetIsOnWithoutNotify(isSavedInBuilding);

            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                OnToggleChanged(index, state);
            });
        }
    }
}