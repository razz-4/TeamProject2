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

        if (factoryUI != null && toggle != null)
        {
            // Add the listener via code to pass this specific GameObject
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    factoryUI.selectDrug(toggleDrugPrefab.drugAssigned.recipe);
                    Debug.Log("Selected");
                }
            });


        }
        else
        {
            Debug.LogError("FactoryUI or Toggle component missing!");
        }
    }
}