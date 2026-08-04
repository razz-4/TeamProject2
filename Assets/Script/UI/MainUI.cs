using UnityEngine;
using UnityEngine.UI;
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
        var shopBtn = root.Q<VisualElement>("Shop-Button");
        var buildingCloseBtn = root.Q<VisualElement>("Close-button");
        var shopCloseBtn = root.Q<VisualElement>("ShopClose-Button");

        // Panels 
        buildingPanel = root.Q<VisualElement>("BuildingPanel-Panel");
        shopPanel = root.Q<VisualElement>("Shop-Panel");

        buildingBtn.RegisterCallback<ClickEvent>(onBuildingBtnPress);
        shopBtn.RegisterCallback<ClickEvent>(onShopBtnPress);
        buildingCloseBtn.RegisterCallback<ClickEvent>(onBuildingCloseBtnPress);
        shopCloseBtn.RegisterCallback<ClickEvent>(onShopCloseBthPress);

        Debug.Log(buildingBtn.ToString());
        Debug.Log(shopBtn.ToString());
    }

    private void onBuildingBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.Flex;
        Debug.Log("Building button clicked");
    }

    private void onBuildingCloseBtnPress(ClickEvent evt)
    {
        buildingPanel.style.display = DisplayStyle.None;
        Debug.Log("Building Close button clicked");
    }
    private void onShopBtnPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.Flex;
        Debug.Log("Shop button clicked");
    }

    private void onShopCloseBthPress(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.None;
        Debug.Log("Shop Close Button Clicked");
    }
}

