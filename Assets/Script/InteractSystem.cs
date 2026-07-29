using UnityEngine;
using UnityEngine.InputSystem;

public class InteractSystem : MonoBehaviour
{
    MobileAction input;

    private void Awake()
    {
        input = new MobileAction();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Mobile.Interact.performed += onInteract;
    }

    private void onInteract(InputAction.CallbackContext ctx)
    {
        shootRaycast();
    }

    private void shootRaycast()
    {
        //VARIABLES
        RaycastHit hit;
        
        //Dont forget to set layer in gameObject
        int layerMask = LayerMask.GetMask("Interactable");
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask,QueryTriggerInteraction.Collide))
        {
            Debug.Log(hit.transform.name);
        }
    }
}
