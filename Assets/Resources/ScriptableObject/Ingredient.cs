using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/Ingredient")]
public class Ingredient : Item
{
    public bool isLocked;
    public bool produce = false;
}
