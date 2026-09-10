using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    [Header("List Visual Trackers")]
    private List<GameObject> drugsPrefabList = new List<GameObject>();
    private List<GameObject> ingredientPrefabList = new List<GameObject>();
    private List<Toggle> toggleButtons = new List<Toggle>();

    [Header("Prefabs")]
    public GameObject drugsPrefab;
    public GameObject ingredientPrefab;

    [Header("UI Containers")]
    [SerializeField] private Transform scrollMenu;
    [SerializeField] private Transform ingredientList;
    [SerializeField] public Toggle sytesizeButton;
    [SerializeField] private ToggleGroup toggleGroup;

    [HideInInspector] public Drug currentDrug;

    private GameObject currentFactoryObj;
    private Factory currentFactoryScript;
    private int currentDrugindex = -1;
    private IngredientRequirement[] currentIngredientRequirement;
    private DrugToggle currentToggle;

    // Static memory of the globally last selected DrugToggle across panel open/closes
    public static DrugToggle lastSelectedToggle;

    #region Unity Callbacks

    private void OnEnable()
    {
        RestoreLastSelection();
    }

    private void Start()
    {
        SpawnDrugListUI();

        sytesizeButton.onValueChanged.RemoveAllListeners();
        sytesizeButton.onValueChanged.AddListener(OnSynthesizeToggleChanged);

        // Restore selection if Start runs after OnEnable on initialization
        RestoreLastSelection();
    }

    private void RestoreLastSelection()
    {
        // 1. Re-select the last chosen toggle if it exists
        if (lastSelectedToggle != null)
        {
            Toggle toggleComp = lastSelectedToggle.GetComponent<Toggle>();
            if (toggleComp != null)
            {
                toggleComp.isOn = true;
                return;
            }
        }

        // 2. Fallback to index 0 if nothing was ever selected before
        if (toggleButtons != null && toggleButtons.Count > 0 && toggleButtons[0] != null)
        {
            toggleButtons[0].isOn = true;
        }
    }

    private void OnSynthesizeToggleChanged(bool state)
    {
        SoundManager.PlaySound(SoundType.UI, 1);

        if (state)
        {
            Debug.Log("Start");
            if (currentToggle == null)
            {
                Debug.LogError("Current Toggle is Null");
                return;
            }

            currentToggle.isProducing = true;

            if (currentFactoryScript != null)
            {
                currentFactoryScript.SelectDrugToProduce(currentDrug, currentToggle.ID);
            }

            Debug.Log(currentToggle.isProducing);
        }
        else
        {
            if (currentToggle != null)
            {
                currentToggle.isProducing = false;
            }

            if (currentFactoryScript != null && currentToggle != null)
            {
                currentFactoryScript.CancelDrug(currentDrug, currentToggle.ID);
            }

            Debug.Log(currentToggle != null ? currentToggle.isProducing : false);
        }
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
        toggleButtons.Clear();

        foreach (Drug drug in ItemManager.Instance.drugs)
        {
            if (drug != null)
            {
                GameObject spawnedDrug = Instantiate(drugsPrefab, scrollMenu);
                ToggleDrugPrefab currentDrugPrefab = spawnedDrug.GetComponent<ToggleDrugPrefab>();
                Toggle toggleComponent = spawnedDrug.GetComponent<Toggle>();

                toggleButtons.Add(toggleComponent);
                drugsPrefabList.Add(spawnedDrug);

                currentDrugPrefab.drugAssigned = drug;
            }
        }

        for (int i = 0; i < toggleButtons.Count; i++)
        {
            DrugToggle drugToggle = drugsPrefabList[i].GetComponent<DrugToggle>();
            drugToggle.ID = i;
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient != null)
            {
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
        foreach (GameObject ingredientObj in ingredientPrefabList)
        {
            ingredientObj.SetActive(false);
        }

        currentIngredientRequirement = ingredientRequirement;
        currentToggle = toggle;

        // Remember this toggle globally as the last selected toggle
        if (toggle != null)
        {
            lastSelectedToggle = toggle;
            if (toggle.TryGetComponent(out ToggleDrugPrefab prefab))
            {
                currentDrug = prefab.drugAssigned;
            }
        }
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

        // Restore factory state for all items
        for (int i = 0; i < drugsPrefabList.Count; i++)
        {
            DrugToggle drugToggle = drugsPrefabList[i].GetComponent<DrugToggle>();
            bool isSavedInBuilding = currentFactoryScript.selectedDrugs.Contains(drugToggle.ID);
            drugToggle.isProducing = isSavedInBuilding;
        }

        // Re-apply selection to the remembered toggle
        RestoreLastSelection();
    }

    private void ProduceItem()
    {
        ItemManager.Instance.synthesize(currentDrug);
    }

    #endregion
}