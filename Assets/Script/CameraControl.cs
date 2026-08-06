using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{

    MobileAction inputs;
    private bool moving;
    [SerializeField] float moveSpeedx = 1.0f;
    [SerializeField] float moveSpeedy = 1.0f;

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
        inputs.Mobile.Pressed.started += _ => moving = true;
        inputs.Mobile.Pressed.canceled += _ => moving = false;

    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    void moveCamera()
    {

        //CAMERA STILL KEEP MOVING EVEN IF NOT DRAGGING
        if (moving)
        {
            Vector2 move = inputs.Mobile.MoveDelta.ReadValue<Vector2>();
            if (move != Vector2.zero)
            {
                //Get the percentage of screen rather than pixel units
                move.x /= Screen.width;
                move.y /= Screen.height;

                //Move opposite to where your finger swipe
                transform.position -= transform.right.normalized * move.x * moveSpeedx;
                transform.position -= transform.forward.normalized * move.y * moveSpeedy;
            }
            Debug.Log(move);
        }
    }




    void Update()
    {
        moveCamera();
    }




}
