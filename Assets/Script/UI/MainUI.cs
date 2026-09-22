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

    //Variables for TutorialManager
    private bool usedOnce = false;
    private bool dialogPlayed = false;
    void Start()
    {
        root = uiDocument.rootVisualElement;

        buildingPreload = Utils.GetAllBuildingInstances<Building>();
        buildingPreload = buildingPreload.OrderBy(p => p.id).ToArray();

        Label cashLabel = root.Q<Label>("Cash");
        cashLabel.dataSource = GameManager.Instance;

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
        // Check if player has enough gold first.
        if (!GameManager.Instance.SpendGold(building.cost))
        {
            return;
        }

        // Spawn the building.
        GameObject spawnedBuilding = Instantiate(building.buildingPrefab,Vector3.zero,building.buildingPrefab.transform.rotation);

        // Get BuildingControl.
        BuildingControl buildingControl = spawnedBuilding.GetComponent<BuildingControl>();

        if (buildingControl == null)
        {
            Debug.LogError("Spawned building does not have BuildingControl!");

            Destroy(spawnedBuilding);
            return;
        }

        if (BuildGrid.Instance == null)
        {
            Debug.LogError("BuildGrid.Instance is null!" );

            Destroy(spawnedBuilding);
            return;
        }

        // Enter building/edit mode.
        BuildingControl.IsBuildingMode = true;

        // Select the newly spawned building.
        // This also starts its black pulsing glow.
        BuildingModeControl.SelectNewBuilding(buildingControl);

        SoundManager.PlaySound(SoundType.BUILD);

        Debug.Log("Spawned and selected building: " + spawnedBuilding.name );
    }


    private void onBuildingBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.Flex;
        SoundManager.PlaySound(SoundType.UI);

        if (!usedOnce) 
        {
            TutorialManager.Instance.GlowingRingPressed();
            usedOnce = true;
        }
    }

    private void onBuildingCloseBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.None;
        SoundManager.PlaySound(SoundType.UI);

        //Tutorial part
        if (!dialogPlayed) { TutorialManager.Instance.GoNextDialog(); dialogPlayed = true; }
    }
    private void onShopBtnPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.Flex;
        SoundManager.PlaySound(SoundType.UI);
    }

    private void onShopCloseBthPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.None;
        SoundManager.PlaySound(SoundType.UI);
    }
}

