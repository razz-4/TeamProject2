using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class GreenHouse : BuildingClass
{
    public int maxToggle;

    // Save index positions instead of object references
    public List<int> selectedIndices = new List<int>(); //{0,5}

    private void Awake()
    {
        buildingType = "GreenHouse";
        placable = true;
        level = 1;
        coinNeededForUpgrade = 10;
        isLock = false;
    }

    public void ProductIngredients(Ingredient targetIngredient, int index)
    {
        if (!selectedIndices.Contains(index))
        {
            selectedIndices.Add(index);
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == targetIngredient)
            {
                ingredient.currentPerSecond += ingredient.perSecond;
            }
        }
    }

    public void CancelProductIngredients(Ingredient targetIngredient, int index)
    {
        if (selectedIndices.Contains(index))
        {
            selectedIndices.Remove(index);
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == targetIngredient)
            {
                ingredient.currentPerSecond -= ingredient.perSecond;
            }
        }
    }
}
