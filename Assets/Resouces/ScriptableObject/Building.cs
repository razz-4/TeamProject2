using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObject/Building")]
public class Building : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public string id;
}
