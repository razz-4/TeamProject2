using UnityEditor.Rendering.Universal;
using UnityEngine;

public class GreenHouse : BuildingClass
{
    [SerializeField] Ingredient[] ingredients;

    private void Awake()
    {
        buildingType = "GreenHouse";
        placable = true;
        level = 1;
        isLock = false;
    }

    void openUI()
    {

    }
}
