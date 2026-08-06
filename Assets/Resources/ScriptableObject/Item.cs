using UnityEngine;
using UnityEngine.UI;


public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string id;
    public int quantity;
    public int value;
    public int perSecond;
}
