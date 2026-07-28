using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainUI : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    private VisualElement root;


    // Panels
    private VisualElement buildingPanel;
    private VisualElement shopPanel;

    void Start()
    {
        root = uiDocument.rootVisualElement;

        // Buttons
        var buildingBtn = root.Q<VisualElement>("Building-button");
        var shopBtn = root.Q<VisualElement>("ShopButtonPanel-Panel");
        var closeBtn = 

        // Panels 
        buildingPanel = root.Q<VisualElement>("BuildingPanel-Panel");
        shopPanel = root.Q<VisualElement>("Shop-Panel");

        buildingBtn.RegisterCallback<ClickEvent>(onBuildingBtnPress);
        shopBtn.RegisterCallback<ClickEvent>(onShopBtnPress);
    }

   
    void Update()
    {
        
    }

    private void onBuildingBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.Flex;
    }

    private void onShopBtnPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.Flex;
    }
}
