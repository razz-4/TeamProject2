using DevelopersHub.ClashOfWhatecer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractSystem : MonoBehaviour
{
    MobileAction input;
    GameObject buildingSelected = null;
    [SerializeField] UIBuildingManager UIBuildingManagerComp;
    [SerializeField] ItemList itemList;
    [SerializeField] FactoryUI factoryUI;
    [SerializeField] GreenHouseUI greenHouseUI;

    private void Awake()
    {
        input = new MobileAction();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Mobile.Interact.performed += onInteract;
    }

    private void OnDisable()
    {
        input.Mobile.Interact.performed -= onInteract;
        input.Disable();
    }

    private void onInteract(InputAction.CallbackContext ctx)
    {
        if (BuildingControl.IsBuildingMode) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            {
                return;
            }
        }

        shootRaycast();
    }

    private void shootRaycast()
    {
        RaycastHit hit;
        int layerMask = Physics.DefaultRaycastLayers;
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            int interactableLayer = LayerMask.NameToLayer("Interactable");
            if (hit.collider.gameObject.layer == interactableLayer)
            {
                buildingSelected = hit.collider.gameObject;
                BuildingClass building = buildingSelected.GetComponent<BuildingClass>();
                string currentBuildingType = building.buildingType;

                UIBuildingManagerComp.setBuildingClass(building);
                UIBuildingManagerComp.ShowUI(currentBuildingType);
                Debug.Log(building);

                itemList.CheckGreenHouse(buildingSelected, currentBuildingType);
                factoryUI.checkFactoryUI(buildingSelected, currentBuildingType);

                GreenHouse gh = building as GreenHouse;
                if (gh != null)
                {
                    greenHouseUI.SetGreenHouse(gh);
                }
            }
            else
            {
                buildingSelected = null;
                UIBuildingManagerComp.HideUI();
            }
        }
    }

    public void EnterBuildingEditMode()
    {
        if (buildingSelected == null) return;

        BuildingControl bc = buildingSelected.GetComponent<BuildingControl>();
        if (bc != null)
        {
            bc.EnterEditMode();
            UIBuildingManagerComp.HideUI();
        }
    }

    public void deleteBuilding()
    {
        if (buildingSelected == null) return;

        BuildingClass building = buildingSelected.GetComponent<BuildingClass>();
        if (building != null)
        {
            int refundAmount = building.price;
            GameManager.Instance.AddGold(refundAmount);
        }

        Destroy(buildingSelected);
        UIBuildingManagerComp.HideUI();
    }
}