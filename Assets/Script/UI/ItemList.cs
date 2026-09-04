using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    Toggle[] toggleButtons;

    public int maxToggle;
    int currentToggleActive = 0;

    public GameObject Greenhouse;
    GreenHouse greenHouseScript;

    // Track the currently targeted greenhouse instance
    private GameObject currentGreenhouseObj;

    private void Start()
    {
        toggleButtons = GetComponentsInChildren<Toggle>(true);
    }

    private void OnToggleChanged(int index, bool state)
    {
        if (greenHouseScript == null) return;

        Ingredient targetIngredient = ItemManager.Instance.ingredients[index];

        if (state)
        {
            if (currentToggleActive < maxToggle)
            {
                currentToggleActive++;
                greenHouseScript.ProductIngredients(targetIngredient, index);
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
                currentToggleActive--;
                greenHouseScript.CancelProductIngredients(targetIngredient, index);
                Debug.Log($"[Building: {currentGreenhouseObj.name}] Removed index {index}: {targetIngredient.name}");
            }
        }
    }

    public void CheckGreenHouse(GameObject greenHouse,string currentBuildingType)
    {
        //Safety check
        if (currentBuildingType != "GreenHouse") return;

        if (greenHouse == null)
        {
            Debug.LogError("Passed Greenhouse GameObject is null!");
            return;
        }

        // 1. Same building selected again -> do nothing
        if (currentGreenhouseObj == greenHouse)
        {
            Debug.Log("Same building re-selected. Keeping current state.");
            return;
        }

        // 2. New building selected -> update target reference
        currentGreenhouseObj = greenHouse;
        greenHouseScript = greenHouse.GetComponent<GreenHouse>();

        if (toggleButtons == null || toggleButtons.Length == 0)
        {
            toggleButtons = GetComponentsInChildren<Toggle>(true);
        }

        // Set local toggle count to this building's saved list count
        currentToggleActive = greenHouseScript.selectedIndices.Count;

        // 3. Rebind and refresh UI for each toggle
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;

            // Remove old listeners so opening UI doesn't re-trigger events
            toggleButtons[i].onValueChanged.RemoveAllListeners();

            // Check if THIS greenhouse saved this index
            bool isSavedInBuilding = greenHouseScript.selectedIndices.Contains(index);
            toggleButtons[i].SetIsOnWithoutNotify(isSavedInBuilding);

            // Add new listener for UI clicks
            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                OnToggleChanged(index, state);
            });
        }
    }
}