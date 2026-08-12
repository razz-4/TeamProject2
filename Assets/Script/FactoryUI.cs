using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    [Header("List of Prefabs")]
    private List<GameObject> drugsPrefabList = new List<GameObject>(); //List of drug prefab just in case
    private List<GameObject> ingredientPrefabList = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject drugsPrefab;
    public GameObject ingredientPrefab;

    [Header("For Scroll Menu")]
    [SerializeField] private Transform scrollMenu; // Assign ItemListFactory directly in Inspector
    [SerializeField] private ToggleGroup toggleGroup;

    [Header("For Ingredient List")]
    [SerializeField] private Transform ingredientList;

    private void Start()
    {
        SpawnDrugListUI();
    }

    private void SpawnDrugListUI()
    {
        if (drugsPrefab == null || scrollMenu == null)
        {
            Debug.LogError("FactoryUI: Missing inspectable references!");
            return;
        }

        if (ItemManager.Instance == null || ItemManager.Instance.drugs == null)
        {
            Debug.Log("Null");
            return;
        }

        drugsPrefabList.Clear();


        foreach (Drug drug in ItemManager.Instance.drugs)
        {
            if (drug != null)
            { 

                //Get reference for each toggle button (LOGIC ERROR ONLY GET FIRST)
                GameObject spawnedDrug = Instantiate(drugsPrefab, scrollMenu);
                ToggleDrugPrefab currentDrugPrefab = spawnedDrug.GetComponent<ToggleDrugPrefab>();

                //assign drugs scriptable object into toggle button
                currentDrugPrefab.drugAssigned = drug;

                drugsPrefabList.Add(spawnedDrug);
            }
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if(ingredient != null)
            {
                //Preload all ingredient for reusing
                GameObject spawnedIngredient = Instantiate(ingredientPrefab, ingredientList);
                IngredientQuantity currentIngredientPrefab = spawnedIngredient.GetComponent<IngredientQuantity>();

                currentIngredientPrefab.ingredient = ingredient;
                ingredientPrefabList.Add(spawnedIngredient);
                spawnedIngredient.SetActive(false);
            }
        }
    }
    
}
