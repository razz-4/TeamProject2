using UnityEngine;

[CreateAssetMenu(fileName = "Drug", menuName = "ScriptableObject/Drug")]
public class Drug : Item
{
    public IngredientRequirement[] recipe;
    public int unlockLevel = 1;
}
