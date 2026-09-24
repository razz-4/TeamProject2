using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GreenHouseUI : MonoBehaviour
{
    [Header("List")]
    private List<GameObject> ingredientPrefabList = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject ingredientPrefab;

    [SerializeField] private Transform itemList;

    private GreenHouse currentGreenHouse;

    private void Awake()
    {

    }

    private void Start()
    {
        SpawnIngredientList();
        gameObject.SetActive(false);
        
    }
    void SpawnIngredientList()
    {
        if (ingredientPrefabList.Count > 0) return;

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            GameObject spawnedIngredients =
                Instantiate(ingredientPrefab, itemList);

            ToggleIngredientPrefab currentIngredientPrefab =
                spawnedIngredients.GetComponent<ToggleIngredientPrefab>();

            currentIngredientPrefab.SetupIngredient(ingredient);

            ingredientPrefabList.Add(spawnedIngredients);
        }
    }

    public void SetGreenHouse(GreenHouse greenhouse)
    {
        currentGreenHouse = greenhouse;
        Debug.Log($"SetGreenHouse called, level = {greenhouse.level}");
        RefreshAllLockStates();
    }

    public void RefreshAllLockStates()
    {
        if (currentGreenHouse == null) return;

        foreach (GameObject prefab in ingredientPrefabList)
        {
            prefab.GetComponent<ToggleIngredientPrefab>()?.RefreshLockState(currentGreenHouse.level);
        }
    }


}
