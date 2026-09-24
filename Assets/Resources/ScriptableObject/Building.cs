using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObject/Building")]
public class Building : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public int upgradeCost;
    public int level;
    public string id;
    public GameObject buildingPrefab;
    

    public static bool IsBeingDragged { get; internal set; }
}
