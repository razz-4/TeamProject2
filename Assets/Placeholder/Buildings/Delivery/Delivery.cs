using UnityEngine;

public class Delivery : BuildingClass
{
    private void Awake()
    {
        buildingType = "Delivery";
        placable = true;
        level = 1;
        isLock = false;
    }
}
