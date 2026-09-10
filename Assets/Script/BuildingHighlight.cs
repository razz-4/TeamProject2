using UnityEngine;

public class BuildingHighlight : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        // Cache or add the Outline component on startup
        if (!TryGetComponent(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false;
    }

    public void EnableHighlight()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableHighlight()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetHighlight(bool state)
    {
        if (state) EnableHighlight();
        else DisableHighlight();
    }
}