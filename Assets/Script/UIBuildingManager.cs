using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;



public class UIBuildingManager : MonoBehaviour
{
    // 1. Static reference to the single instance
    public static UIBuildingManager Instance;

    [SerializeField] private GameObject BuildingUI;
    [SerializeField] private GameObject Canva;
    [SerializeField] private UnityEngine.UI.Image SpecialButtonImage;
    [SerializeField] private UIDocument DeliveryUI;
    [SerializeField] private Sprite[] specialButtonImageList;

    [Header ("Level UP Button")]
    [SerializeField] private LevelUpButton levelUpButtonScript;
    [SerializeField] private TextMeshProUGUI levelUpAmount;

    private VisualElement root;
    private VisualElement DeliveryUIPanel;
    private GameObject specialButton;
    private GameObject levelUpButton;
    private GameObject GreenHouseUI;
    private GameObject FactoryUI;

    private string currentBuildingType;
    private BuildingClass currentBuildingClass;
 

    //Tutorial
    private bool TutorialDone = false;

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
    private void Start()
    {
        root = DeliveryUI.rootVisualElement;
        DeliveryUIPanel = root.Q<VisualElement>("DeliveryUIPanel");
        DeliveryUIPanel.AddToClassList("SettingUp");
    }
    public void HideUI()
    {
        BuildingUI.SetActive(false);
        GreenHouseUI.SetActive(false);
        FactoryUI.SetActive(false);
        if (specialButton != null) specialButton.SetActive(false);
        if(currentBuildingType != null)
        {
            SoundManager.PlaySound(SoundType.UI, 1);
        }
        currentBuildingType = null;
    }

    public void HideBuildingUI()
    {
        BuildingUI.SetActive(false);
        if (specialButton != null) specialButton.SetActive(false);
        if (currentBuildingType != null)
        {
            SoundManager.PlaySound(SoundType.UI, 1);
        }
        currentBuildingType = null;
    }

    public void ShowUI(string buildingType)
    {
        BuildingUI.SetActive(true);
        SoundManager.PlaySound(SoundType.UI, 1);
        currentBuildingType = buildingType;

        if (!string.IsNullOrEmpty(currentBuildingType))
        {
            if (specialButton != null) specialButton.SetActive(true);
            if (levelUpButton != null) levelUpButton.SetActive(true);
            SetSpecialButton(currentBuildingType);
            UpdateLevelUpUI();
        }
        else
        {
            if (specialButton != null) specialButton.SetActive(false);
            if (levelUpButton != null) levelUpButton.SetActive(false);
        }

        //Tutrorial section
        if (!TutorialDone) { TutorialManager.Instance.SetGlowRingPosition(1); TutorialDone = true; }
    }

    //Change the button Text
    private void SetSpecialButton(string buildingType)
    {
        switch (buildingType)
        {
            case "GreenHouse":
                SpecialButtonImage.sprite = specialButtonImageList[0];
                break;

            case "Delivery":
                SpecialButtonImage.sprite = specialButtonImageList[2];
                break;

            case "Factory":
                SpecialButtonImage.sprite = specialButtonImageList[1];
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
                DeliveryUIPanel.RemoveFromClassList("SettingUp");

                //Add Delivery menu...
                break;

            case "Factory":
                FactoryUI.SetActive(true);
                break;

            default:
                break;
        }
    }

    //Set building type that is selected
    public void setBuildingClass(BuildingClass buildingClass)
    {
        currentBuildingClass = buildingClass;
        levelUpButtonScript.SetCurrentBuildingClass(buildingClass);
        Debug.Log(buildingClass);

    }

    public void UpdateLevelUpUI()
    {
        if (currentBuildingClass == null) return;
        levelUpAmount.text = $"{currentBuildingClass.coinNeededForUpgrade}";
    }
}