using System.Collections.Generic;
using System.Linq;
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

    IngredientRequirement[] currentIngredientRequirement;
    private int count = 0;

    private void Start()
    {
        SpawnDrugListUI();
    }

    private void Update()
    {
        showIngredientList();
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
                Toggle toggleComponent = spawnedDrug.GetComponent<Toggle>();
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
    
    public void selectDrug(IngredientRequirement[] ingredientRequirement)
    {
        currentIngredientRequirement = ingredientRequirement;
    }

    public void showIngredientList()
    {
        if (currentIngredientRequirement == null) return;

        for (int i = 0; i < ingredientPrefabList.Count; i++)
        {
            GameObject listObj = ingredientPrefabList[i];
            IngredientQuantity ingredientQuantity = listObj.GetComponent<IngredientQuantity>();

            string ingredientName = ingredientQuantity.ingredient.name;
            
            for(int j = 0; j < currentIngredientRequirement.Length; j++)
            {
                string ingredientRequirement = currentIngredientRequirement[j].ingredient.name;
                if(ingredientName == ingredientRequirement)
                {
                    listObj.SetActive(true);
                }

            }
        }
    }
}
