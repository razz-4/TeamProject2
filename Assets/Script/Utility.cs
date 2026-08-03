using UnityEngine;

public static class Utils
{
    public static T[] GetAllIngredientInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("SceriptableObject/Ingredient");
    }

    public static T[] GetAllDrugInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("SceriptableObject/Drug");
    }
}