using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DevelopersHub.ClashOfWhatecer;

public class CameraControl : MonoBehaviour
{
    MobileAction inputs;
    private bool moving;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeedx = 1.0f;
    [SerializeField] float moveSpeedy = 1.0f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float cameraRadius = 0.5f;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] float zoomSpeed = 0.05f;

    [Header("References")]
    [SerializeField] CinemachineCamera cinemachine;

    private void Awake()
    {
        inputs = new MobileAction();
        moving = false;
    }

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Mobile.Pressed.started += _ => CheckPointer();
        inputs.Mobile.Pressed.canceled += _ => moving = false;
        inputs.Mobile.Scroll.performed += ctx => ZoomCamera(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Check active touch instances on mobile
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed && EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue()))
                {
                    return true;
                }
            }
        }

        // Fallback for Mouse (PC / Unity Editor)
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CheckPointer()
    {
        // Don't enable camera drag if touching UI
        if (IsPointerOverUI())
        {
            moving = false;
            return;
        }

        moving = true;
    }

    private void ZoomCamera(Vector2 scrollDelta)
    {
        if (IsPointerOverUI()) return;

        float zoomAmount = scrollDelta.y;
        float newSize = cinemachine.Lens.OrthographicSize - (zoomAmount * zoomSpeed);
        cinemachine.Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    private void PinchZoom()
    {
        if (Touchscreen.current == null) return;

        var touchscreen = Touchscreen.current;
        var touch0 = touchscreen.touches[0];
        var touch1 = touchscreen.touches[1];

        if (!touch0.press.isPressed || !touch1.press.isPressed) return;

        // Abort pinch zoom if any finger is touching UI
        if (IsPointerOverUI()) return;

        Vector2 position0 = touch0.position.ReadValue();
        Vector2 position1 = touch1.position.ReadValue();

        float currentDistance = Vector2.Distance(position0, position1);

        Vector2 previousPosition0 = position0 - touch0.delta.ReadValue();
        Vector2 previousPosition1 = position1 - touch1.delta.ReadValue();

        float previousDistance = Vector2.Distance(previousPosition0, previousPosition1);
        float delta = currentDistance - previousDistance;

        float newSize = cinemachine.Lens.OrthographicSize - (delta * zoomSpeed);
        cinemachine.Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    private void MoveCamera()
    {
        if (BuildingControl.IsBeingDragged)
        {
            return;
        }

        if (moving)
        {
            // Safeguard: stop moving if dragging onto UI mid-gesture
            if (IsPointerOverUI())
            {
                moving = false;
                return;
            }

            Vector2 move = inputs.Mobile.MoveDelta.ReadValue<Vector2>();
            if (move != Vector2.zero)
            {
                move.x /= Screen.width;
                move.y /= Screen.height;

                float currentZoom = cinemachine.Lens.OrthographicSize;
                Vector3 moveDirection = (-transform.right * move.x * moveSpeedx * currentZoom) + (-transform.forward * move.y * moveSpeedy * currentZoom);

                float distance = moveDirection.magnitude;

                if (!Physics.SphereCast(transform.position, cameraRadius, moveDirection.normalized, out RaycastHit hit, distance, wallLayer))
                {
                    transform.position += moveDirection;
                }
            }
        }
    }

    private void Update()
    {
        // Active 2-finger check for mobile pinch zoom
        if (Touchscreen.current != null &&
            Touchscreen.current.touches.Count >= 2 &&
            Touchscreen.current.touches[0].press.isPressed &&
            Touchscreen.current.touches[1].press.isPressed)
        {
            moving = false; // Disable camera panning during pinch
            PinchZoom();
        }
        else
        {
            MoveCamera();
        }
    }
}