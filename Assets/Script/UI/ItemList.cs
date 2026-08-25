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

/*
 *  The current issue happens because CheckGreenHouse resets currentToggleActive = 0 and forces all UI toggles to SetIsOnWithoutNotify(false) every time it runs—even when re-selecting the exact same building.

To save the toggle state for the same building while resetting toggles when switching to a different building, track the active building instance and skip the state reset when CheckGreenHouse is called with the same reference.

Here is the updated script with the fix:
C#

using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    Button[] buttons;
    Toggle[] toggleButtons;

    public int maxToggle;
    int currentToggleActive = 0;

    public GameObject Greenhouse;
    GreenHouse greenHouseScript;

    // Track the currently targeted greenhouse instance
    private GameObject currentGreenhouseObj;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }

    private void OnToggleChanged(int index, bool state)
    {
        if (state)
        {
            // Check if limit is reached
            if (currentToggleActive < maxToggle)
            {
                currentToggleActive++;
                greenHouseScript.ProductIngredients(ItemManager.Instance.ingredients[index]);
                Debug.Log("Added: " + ItemManager.Instance.ingredients[index].name);
            }
            else
            {
                toggleButtons[index].SetIsOnWithoutNotify(false);
                Debug.Log("Toggle limit reached! Max allowed: " + maxToggle);
            }
        }
        else
        {
            if (currentToggleActive > 0)
            {
                currentToggleActive--;
                greenHouseScript.CancelProductIngredients(ItemManager.Instance.ingredients[index]);
                Debug.Log("Removed: " + ItemManager.Instance.ingredients[index].name);
            }
        }
    }

    public void CheckGreenHouse(GameObject greenHouse)
    {
        if (greenHouse == null)
        {
            Debug.LogError("Passed Greenhouse GameObject is null!");
            return;
        }

        // If selecting the exact same building again, keep the toggles intact
        if (currentGreenhouseObj == greenHouse)
        {
            Debug.Log("Same building re-selected. Keeping current toggle states.");
            return;
        }

        // New building selected: Update reference and reset states
        currentGreenhouseObj = greenHouse;
        greenHouseScript = greenHouse.GetComponent<GreenHouse>();
        currentToggleActive = 0;

        toggleButtons = GetComponentsInChildren<Toggle>(true);
        Debug.Log($"Found {toggleButtons.Length} Toggle buttons in hierarchy.");

        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;

            toggleButtons[i].onValueChanged.RemoveAllListeners();
            toggleButtons[i].SetIsOnWithoutNotify(false);

            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                Debug.Log($"Button pressed on index: {index}, state: {state}");
                OnToggleChanged(index, state);
            });
        }
    }
}

Key Changes:

    Added a currentGreenhouseObj field to track the reference of the active target.

    Check if currentGreenhouseObj == greenHouse in CheckGreenHouse(). If it is the same building, it skips resetting the listener callbacks, currentToggleActive counter, and UI visuals.

    When a genuinely new building reference is provided, it updates currentGreenhouseObj and executes the full reset sequence.
 * */