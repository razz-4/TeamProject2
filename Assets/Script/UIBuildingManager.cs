using UnityEngine;
using TMPro;

public class UIBuildingManager : MonoBehaviour
{
    // 1. Static reference to the single instance
    public static UIBuildingManager Instance;

    [SerializeField] private GameObject BuildingUI;
    [SerializeField] private GameObject Canva;
    [SerializeField] private TextMeshProUGUI SpecialButtonText;

    private GameObject specialButton;
    private GameObject levelUpButton;
    private GameObject GreenHouseUI;
    private GameObject FactoryUI;

    private string currentBuildingType;
    private BuildingClass currentBuildingClass;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            // Destroy duplicate if one already exists
            Destroy(gameObject);
            return;
        }

        Instance = this;

        
        if (BuildingUI != null)
        {
            Transform specialBtnTransform = BuildingUI.transform.Find("SpecialButton");
            Transform lvlUpBtnTransform = BuildingUI.transform.Find("LevelUpButton");
            if (specialBtnTransform != null)
            {
                specialButton = specialBtnTransform.gameObject;
                levelUpButton = lvlUpBtnTransform.gameObject;
            }
        }

        if (Canva != null)
        {
            GreenHouseUI = Canva.transform.Find("GreenHouseUI").gameObject;
            FactoryUI = Canva.transform.Find("FactoryUI").gameObject;

            GreenHouseUI.SetActive(true);
        }
    }

    public void HideUI()
    {
        BuildingUI.SetActive(false);
        GreenHouseUI.SetActive(false);
        FactoryUI.SetActive(false);
        if (specialButton != null) specialButton.SetActive(false);
        currentBuildingType = null;
    }

    public void ShowUI(string buildingType)
    {
        BuildingUI.SetActive(true);
        currentBuildingType = buildingType;

        if (!string.IsNullOrEmpty(currentBuildingType))
        {
            if (specialButton != null) specialButton.SetActive(true);
            if (levelUpButton != null) levelUpButton.SetActive(true);
            SetSpecialButton(currentBuildingType);
        }
        else
        {
            if (specialButton != null) specialButton.SetActive(false);
            if (levelUpButton != null) levelUpButton.SetActive(false);
        }
    }

    //Change the button Text
    private void SetSpecialButton(string buildingType)
    {
        switch (buildingType)
        {
            case "GreenHouse":
                SpecialButtonText.text = "Green House";
                break;

            case "Delivery":
                SpecialButtonText.text = "Delivery";
                break;

            case "Factory":
                SpecialButtonText.text = "Factory";
                break;

            default:
                break;
        }
    }

    //Open the UI based on building type
    public void OpenUI()
    {
        //Safety check
        if (string.IsNullOrEmpty(currentBuildingType)) return;

        switch (currentBuildingType)
        {
            case "GreenHouse":
                GreenHouseUI.SetActive(true);
                break;

            case "Delivery":
                Debug.Log("Open Delivery UI");
                //Add Delivery menu...
                break;

            case "Factory":
                FactoryUI.SetActive(true);
                break;

            default:
                break;
        }
    }

    public void upgradeBuilding()
    {
        currentBuildingClass.level++;
        Debug.Log("Building Type:" + currentBuildingClass.buildingType + "Building Level: " + currentBuildingClass.level);
    }
    //Set building type that is selected
    public void setBuildingClass(BuildingClass buildingClass)
    {
        currentBuildingClass = buildingClass;
    }


}