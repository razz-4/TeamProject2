using UnityEngine;

public static class Utils
{
    public static T[] GetAllIngredientInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("ScriptableObject/Ingredient");
    }

    public static T[] GetAllDrugInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("ScriptableObject/Drug");
    }

    public static T[] GetAllBuildingInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("ScriptableObject/Building");
    }

    public static BuyerName[] GetAllBuyerNameInstances()
    {
        return Resources.LoadAll<BuyerName>("ScriptableObject/BuyerName");
    }
}