using DevelopersHub.ClashOfWhatecer;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainUI : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] VisualTreeAsset buildingSlotTemplate;
    private VisualElement root;
    [SerializeField] public BuildGrid _grid = null;

    public Building[] buildingPreload;
    // Panels
    private VisualElement buildingPanel;
    private VisualElement shopPanel;

    private ScrollView buildingScrollView;
    void Start()
    {
        root = uiDocument.rootVisualElement;

        buildingPreload = Utils.GetAllBuildingInstances<Building>();
        buildingPreload = buildingPreload.OrderBy(p => p.id).ToArray();

        // Buttons
        var buildingBtn = root.Q<VisualElement>("Building-button");
        var shopBtn = root.Q<VisualElement>("Shop-Button");
        var buildingCloseBtn = root.Q<VisualElement>("Close-button");
        var shopCloseBtn = root.Q<VisualElement>("ShopClose-Button");

        // Panels 
        buildingPanel = root.Q<VisualElement>("BuildingPanel-Panel");
        shopPanel = root.Q<VisualElement>("Shop-Panel");

        buildingScrollView = root.Q<ScrollView>("Building-ScrollView");

        buildingBtn.RegisterCallback<ClickEvent>(onBuildingBtnPress);
        shopBtn.RegisterCallback<ClickEvent>(onShopBtnPress);
        buildingCloseBtn.RegisterCallback<ClickEvent>(onBuildingCloseBtnPress);
        shopCloseBtn.RegisterCallback<ClickEvent>(onShopCloseBthPress);

        setupScrollView(buildingScrollView, buildingPreload);
    }

    private void setupScrollView<T>(ScrollView scrollView, T[] buildings) where T : Building
    {
        if (buildings == null || scrollView == null) return;

        foreach (Building building in buildings)
        {
            VisualElement slotInstance = buildingSlotTemplate.Instantiate();
            var slotUI = new BuildingSlotUI(slotInstance);
            slotUI.setBuilding(building);

            slotUI.OnSlotClicked += OnBuildingSlotClicked;

            scrollView.Add(slotInstance);
        }

    }

    private void OnBuildingSlotClicked(Building building)
    {
        Debug.Log(building.buildingName);
        var buildingTemplate = building.buildingPrefab;
        buildingTemplate = Instantiate(buildingTemplate, new Vector3(0, 0, 0), Quaternion.Euler(-90,0,0));

        var bc = buildingTemplate.GetComponent<BuildingControl>();
        if (bc != null && BuildGrid.Instance != null)
        {
            
            //bc.PlaceOnGrid(0, 0);
            
            BuildingControl.IsBuildingMode = true;
        }
    }

    private void onBuildingBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.Flex;
    }

    private void onBuildingCloseBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.None;
    }
    private void onShopBtnPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.Flex;
    }

    private void onShopCloseBthPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.None;
    }
}

