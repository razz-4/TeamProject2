using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class BuildingSlotUI
{
    private VisualElement root;

    public Building storedBuilding { get; private set; }

    private Image buildingImage;
    private Label buildingName;
    private Label cost;

    public event Action<Building> OnSlotClicked;

    public BuildingSlotUI(VisualElement root)
    {
        this.root = root;

        buildingImage = root.Q<Image>("BuildingImage");
        buildingName = root.Q<Label>("BuildingName");
        cost = root.Q<Label>("Cost");

        var buildingBtn = root.Q<VisualElement>("BuildingButton");

        buildingBtn.RegisterCallback<ClickEvent>(evt =>
        {
            OnSlotClicked?.Invoke(storedBuilding);
        });
    }

    public void setBuilding(Building building)
    {
        storedBuilding = building;

        buildingName.text = building.buildingName;
        cost.text = building.cost.ToString();


        if (building.icon != null)
        {
            buildingImage.sprite = building.icon;
            
            buildingImage.style.display = DisplayStyle.Flex;
        }
    }
}