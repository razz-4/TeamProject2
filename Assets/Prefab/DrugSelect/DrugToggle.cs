using UnityEngine;
using UnityEngine.UI;

public class DrugToggle : MonoBehaviour
{
    private Toggle toggle;
    private ToggleDrugPrefab toggleDrugPrefab;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggleDrugPrefab = GetComponent<ToggleDrugPrefab>();

        // Find the FactoryUI manager in your scene dynamically
        FactoryUI factoryUI = Object.FindAnyObjectByType<FactoryUI>();
        GameObject toggleGroupObj = GameObject.Find("IngredientGroup");
        ToggleGroup toggleGroup = toggleGroupObj.GetComponent<ToggleGroup>();
        toggle.group = toggleGroup;

        if (factoryUI != null && toggle != null)
        {
            // Add the listener via code to pass this specific GameObject
            toggle.onValueChanged.AddListener((bool state) =>
            {
                if (state)
                {
                    factoryUI.selectDrug(toggleDrugPrefab.drugAssigned.recipe);
                    factoryUI.currentDrug = toggleDrugPrefab.drugAssigned;
                    Debug.Log($"Current Drug: {factoryUI.currentDrug.name}");
                    
                }
                else
                {

                }
            });


        }
        else
        {
            Debug.LogError("FactoryUI or Toggle component missing!");
        }
    }
}