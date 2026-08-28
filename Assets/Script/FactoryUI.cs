using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    [Header("List Visual Trackers")]
    private List<GameObject> drugsPrefabList = new List<GameObject>(); // List of drug prefab just in case
    private List<GameObject> ingredientPrefabList = new List<GameObject>();
    private List<Toggle> toggleButtons = new List<Toggle>();

    [Header("Prefabs")]
    public GameObject drugsPrefab;
    public GameObject ingredientPrefab;

    [Header("UI Containers")]
    [SerializeField] private Transform scrollMenu; // Assign ItemListFactory directly in Inspector
    [SerializeField] private Transform ingredientList;
    [SerializeField] public Toggle sytesizeButton;
    [SerializeField] private ToggleGroup toggleGroup;

    [HideInInspector] public Drug currentDrug;

    private GameObject currentFactoryObj;
    private Factory currentFactoryScript;
    private int currentDrugindex = -1;
    private IngredientRequirement[] currentIngredientRequirement;
    private DrugToggle currentToggle;
    #region Unity Callbacks

    private void Start()
    {
        SpawnDrugListUI();

        sytesizeButton.onValueChanged.AddListener((bool state) =>
        {
            if (state)
            {
                if(currentToggle == null)
                {
                    Debug.LogError("Current Toggle is Null");
                    return;
                }
                EventManager.Instance.generateItems += ProduceItem;

                currentToggle.isProducing = true;
                currentFactoryScript.selectedDrugs.Add(currentToggle.ID);
                Debug.Log(currentToggle.isProducing);

                foreach (int drug in currentFactoryScript.selectedDrugs)
                {
                    Debug.Log(drug);
                }
            }
            else
            {
                currentToggle.isProducing = false;
                currentFactoryScript.selectedDrugs.Remove(currentToggle.ID);
                Debug.Log(currentToggle.isProducing);

                foreach (int drug in currentFactoryScript.selectedDrugs)
                {
                    Debug.Log(drug);
                }
            }
        });
    }

    private void Update()
    {
        showIngredientList();

        if (!currentToggle) return;

        if (currentToggle.isProducing)
        {
            sytesizeButton.SetIsOnWithoutNotify(true);
        }
        else
        {
            sytesizeButton.SetIsOnWithoutNotify(false);
        }
    }

    #endregion

    #region UI Setup & Spawning

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
                // Get reference for each toggle button (LOGIC ERROR ONLY GET FIRST)
                GameObject spawnedDrug = Instantiate(drugsPrefab, scrollMenu);
                ToggleDrugPrefab currentDrugPrefab = spawnedDrug.GetComponent<ToggleDrugPrefab>();
                Toggle toggleComponent = spawnedDrug.GetComponent<Toggle>();

                toggleButtons.Add(toggleComponent);
                drugsPrefabList.Add(spawnedDrug);

                // Assign drugs scriptable object into toggle button
                currentDrugPrefab.drugAssigned = drug;
            }
        }

        for (int i = 0; i < toggleButtons.ToArray().Length; i++)
        {
            DrugToggle drugToggle = drugsPrefabList[i].GetComponent<DrugToggle>();
            drugToggle.ID = i;
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient != null)
            {
                // Preload all ingredient for reusing
                GameObject spawnedIngredient = Instantiate(ingredientPrefab, ingredientList);
                IngredientQuantity currentIngredientPrefab = spawnedIngredient.GetComponent<IngredientQuantity>();

                currentIngredientPrefab.ingredient = ingredient;
                ingredientPrefabList.Add(spawnedIngredient);
                spawnedIngredient.SetActive(false);
            }
        }
    }

    #endregion

    #region Public UI Methods

    public void selectDrug(IngredientRequirement[] ingredientRequirement, DrugToggle toggle)
    {
        // Hide all ingredients currently on screen
        foreach (GameObject ingredientObj in ingredientPrefabList)
        {
            ingredientObj.SetActive(false);
        }

        currentIngredientRequirement = ingredientRequirement;

        foreach (IngredientRequirement ingredientRequirement1 in currentIngredientRequirement)
        {
            Debug.Log(ingredientRequirement1.amount);
        }

        currentToggle = toggle;
        // showIngredientList();
    }

    public void showIngredientList()
    {
        if (currentIngredientRequirement == null) return;

        for (int i = 0; i < ingredientPrefabList.Count; i++)
        {
            GameObject listObj = ingredientPrefabList[i];
            IngredientQuantity ingredientQuantity = listObj.GetComponent<IngredientQuantity>();

            string ingredientName = ingredientQuantity.ingredient.name;

            for (int j = 0; j < currentIngredientRequirement.Length; j++)
            {
                string ingredientRequirement = currentIngredientRequirement[j].ingredient.name;
                if (ingredientName == ingredientRequirement)
                {
                    listObj.SetActive(true);
                    ingredientQuantity.updateTextAmount(currentIngredientRequirement[j]);
                }
            }
        }
    }

public void checkFactoryUI(GameObject building, string currentBuildingType)
    {
        if (currentBuildingType != "Factory" || building == null) return;

        currentFactoryObj = building;
        currentFactoryScript = currentFactoryObj.GetComponent<Factory>();

        // Deselect current toggle UI selection visuals
        if (toggleGroup != null) toggleGroup.SetAllTogglesOff();

        // Restore factory state for all items
        for (int i = 0; i < drugsPrefabList.Count; i++)
        {
            DrugToggle drugToggle = drugsPrefabList[i].GetComponent<DrugToggle>();
            bool isSavedInBuilding = currentFactoryScript.selectedDrugs.Contains(drugToggle.ID);
            drugToggle.isProducing = isSavedInBuilding;
        }

        currentToggle = null;
        currentIngredientRequirement = null;
        sytesizeButton.SetIsOnWithoutNotify(false);
    }
    
    private void ProduceItem()
    {
        ItemManager.Instance.synthesize(currentDrug);
    }

    #endregion
}
