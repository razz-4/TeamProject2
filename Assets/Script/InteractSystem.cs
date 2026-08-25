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
        //if (BuildingControl.IsBuildingMode) return;
        // Check if the tap landed on a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Ignore world interaction if tapping UI
        }

        // For Mobile/Touch support with the new Input System:
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
        //VARIABLES
        RaycastHit hit;

        //Dont forget to set layer in gameObject
        int layerMask = Physics.DefaultRaycastLayers;
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        //Shoots a Raycast and output hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask,QueryTriggerInteraction.Collide))
        {
            //check if raycast hit a interactable
            int interactableLayer = LayerMask.NameToLayer("Interactable");
            if (hit.collider.gameObject.layer == interactableLayer)
            {
                buildingSelected = hit.collider.gameObject;
                BuildingClass building = buildingSelected.GetComponent<BuildingClass>();
                string currentBuildingType = building.buildingType;

                //Set current building into BuildingUIManager
                UIBuildingManagerComp.ShowUI(currentBuildingType);
                UIBuildingManagerComp.setBuildingClass(building);


                itemList.CheckGreenHouse(buildingSelected,currentBuildingType);
                
            }
            else
            {
               buildingSelected = null;
               UIBuildingManagerComp.HideUI();
            }
        }
    }

    public void deleteBuilding()
    {
        Destroy(buildingSelected);
        UIBuildingManagerComp.HideUI();
    }
}
