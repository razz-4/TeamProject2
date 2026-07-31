using UnityEngine;

public static class Utils
{

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("SceriptableObject/Ingredient");
    }
}