using UnityEngine;

public class Factory : BuildingClass
{
    private void Awake()
    {
        buildingType = "Factory";
        placable = true;
        level = 1;
        isLock = false;
    }
}
