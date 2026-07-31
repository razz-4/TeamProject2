using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string name;
    public string id;
    public Sprite icon;
    public bool isLocked;
    public int quantity;
    public int perSecond;
}
