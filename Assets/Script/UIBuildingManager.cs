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
    private GameObject GreenHouseUI;
    private GameObject FactoryUI;

    private string currentBuildingType;

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
            if (specialBtnTransform != null)
            {
                specialButton = specialBtnTransform.gameObject;
            }
        }

        if (Canva != null)
        {
            GreenHouseUI = Canva.transform.Find("GreenHouseUI").gameObject;
            FactoryUI = Canva.transform.Find("FactoryUI").gameObject;
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
            SetSpecialButton(currentBuildingType);
        }
        else
        {
            if (specialButton != null) specialButton.SetActive(false);
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
}