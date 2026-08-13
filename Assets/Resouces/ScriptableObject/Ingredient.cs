using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string name;
    public string id;
    public Sprite icon;
    public bool isLocked;
    public bool produce = false;
    public float quantity;
    public int value;
    public int perSecond;
    public int CurrentPerSecond;
}
