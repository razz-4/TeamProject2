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
}