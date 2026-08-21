using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class GreenHouse : BuildingClass
{


    private void Awake()
    {
        buildingType = "GreenHouse";
        placable = true;
        level = 1;
        isLock = false;

    }

    public void ProductIngredients(Ingredient targetIngredient)
    {
        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == targetIngredient)
            {
                ingredient.currentPerSecond += ingredient.perSecond;
            }
        }
    }

    public void CancelProductIngredients(Ingredient targetIngredient)
    {
        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == targetIngredient)
            {
                ingredient.currentPerSecond -= ingredient.perSecond;
            }
        }
    }
}
