using UnityEngine;

public class UIBuildingManager : MonoBehaviour
{
    [SerializeField] GameObject BuildingUI;
    GameObject specialButton;
    string currentBuildingType;

    private void Awake()
    {
        specialButton = BuildingUI.transform.Find("SpecialButton").gameObject;
    }
    public void hideUI()
    {
        BuildingUI.SetActive(false);
        specialButton.SetActive(false);
        currentBuildingType = null;
    }

    public void showUI(string buildingType)
    {
        BuildingUI.SetActive(true);
        currentBuildingType = buildingType;
        if (currentBuildingType != null)
        {
            specialButton.SetActive(true);
        }
        else
        {
            specialButton.SetActive(false);
        }
    }

    void setSpecialButton(string buildingType)
    {
        
    }
}
