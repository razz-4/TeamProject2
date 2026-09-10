using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            //Finds camera with "MainCamera" Tag
            mainCamera = Camera.main;
        }
    }

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Quaternion rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}
