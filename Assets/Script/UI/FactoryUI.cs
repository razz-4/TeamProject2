using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    public GameObject drugsPrefab;
    public GameObject ingredientPrefab;
    public Transform scrollMenu;
    private List<GameObject> drugsPrefabList = new List<GameObject>();

    [SerializeField] private Transform itemListFactory; // Assign ItemListFactory directly in Inspector
    [SerializeField] private ToggleGroup toggleGroup;

    private void Start()
    {
        SpawnDrugListUI();
    }

    private void SpawnDrugListUI()
    {
        if (drugsPrefab == null || itemListFactory == null)
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
                GameObject spawnedDrug = Instantiate(drugsPrefab, itemListFactory);
                ToggleDrugPrefab currentDrugPrefab = spawnedDrug.GetComponent<ToggleDrugPrefab>();

                //assign drugs scriptable object into toggle button
                currentDrugPrefab.drugAssigned = drug;

                drugsPrefabList.Add(spawnedDrug);
            }
        }
    }
}
