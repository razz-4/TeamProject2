using UnityEngine;

[CreateAssetMenu(fileName = "Drug", menuName = "ScriptableObject/Drug")]
public class Drug : ScriptableObject
{
    public string name;
    public int id;
    public Sprite Icon;
    public int quantity;
    public int perSecond;
    public int value;
    public IngredientRequirement[] recipe;

}
