using UnityEngine;

public class Item : ScriptableObject
{
    public string itemName;
    public string id;
    public Sprite icon;
    public int quantity;
    public int value;
    public int perSecond;
    public int currentPerSecond;
}
