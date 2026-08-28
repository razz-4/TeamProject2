using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DevelopersHub.ClashOfWhatecer;

public class CameraControl : MonoBehaviour
{

    MobileAction inputs;
    private bool moving;

    //Movement Speed
    //moveSpeedx = 11.25 moveSpeedy = 40 
    [SerializeField] float moveSpeedx = 1.0f;
    [SerializeField] float moveSpeedy = 1.0f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] float zoomSpeed;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float cameraRadius = 0.5f;

    [SerializeField] CinemachineCamera cinemachine;
    
    private void Awake()
    {
        //create input Object
        inputs = new MobileAction();
        moving = false;

    }

    private void OnEnable()
    {
        inputs.Enable();

        //moving = true if touched and false if let go
        inputs.Mobile.Pressed.started += _ => checkPointer();
        inputs.Mobile.Pressed.canceled += _ => moving = false;

        inputs.Mobile.Scroll.performed += ctx => zoomCamera(ctx.ReadValue<Vector2>());

    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Check Touchscreen touches (Mobile)
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.primaryTouch;
            return EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue());
        }

        // Fallback for Mouse (PC / Editor)
        return EventSystem.current.IsPointerOverGameObject();
    }

    void checkPointer()
    {
        if (IsPointerOverUI())
        {
            moving = false;
            return;
        }
        else
        {
            moving = true;
        }
    }
    //only for mouse input but not touchscreen
    void zoomCamera(Vector2 scrollDelta)
    {
        float zoomAmount = scrollDelta.y;

        float newSize = cinemachine.Lens.OrthographicSize - (zoomAmount * zoomSpeed);

        // Clamp the size between your min and max bounds
        cinemachine.Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);

    }
    void moveCamera()
    {
        
        if (BuildingControl.IsBeingDragged)
        {
            Debug.Log("Camera blocked by IsBeingDragged = true");
            return;
        }

        //CAMERA STILL KEEP MOVING EVEN IF NOT DRAGGING
        if (moving)
        {
            Vector2 move = inputs.Mobile.MoveDelta.ReadValue<Vector2>();
            if (move != Vector2.zero)
            {
                //Get the percentage of screen rather than pixel units
                move.x /= Screen.width;
                move.y /= Screen.height;

                // Grab the current zoom level to scale our movement
                float currentZoom = cinemachine.Lens.OrthographicSize;

                // Multiply the movement by the currentZoom so it speeds up when zoomed out 
                // and slows down when zoomed in
                Vector3 moveDirection = (-transform.right * move.x * moveSpeedx * currentZoom) + (-transform.forward * move.y * moveSpeedy * currentZoom);

                float distance = moveDirection.magnitude;

                // Only move if there is no wall in the target path
                if (!Physics.SphereCast(transform.position, cameraRadius, moveDirection.normalized, out RaycastHit hit, distance, wallLayer))
                {
                    transform.position += moveDirection;
                }
            }
        }
    }

    void Update()
    {
        moveCamera();
        
    }


}
