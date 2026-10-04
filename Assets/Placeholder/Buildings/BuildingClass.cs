using UnityEngine;

public class BuildingClass : MonoBehaviour
{
    public string buildingType;
    public bool placable;
    public int level;
    public int price;
    public int coinNeededForUpgrade;
    public float upgradeMultiplier;
    public bool isLock;

    public virtual void LevelUp()
    {
        Debug.Log("LevelUp");
    }
}
