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

    private void Start()
    {
        SpawnIngredientList();
    }
    void SpawnIngredientList()
    {
        foreach(Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            GameObject spawnedIngredients = Instantiate(ingredientPrefab, itemList);
            ToggleIngredientPrefab currentIngredientPrefab = spawnedIngredients.GetComponent<ToggleIngredientPrefab>();
            Toggle toggleComponent = spawnedIngredients.GetComponent<Toggle>();

            //Assign Ingredient to prefab
            currentIngredientPrefab.ingredientAssigned = ingredient;

            ingredientPrefabList.Add(spawnedIngredients);
        }
    }
}
