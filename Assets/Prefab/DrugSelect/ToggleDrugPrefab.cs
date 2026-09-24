using UnityEngine;
using UnityEngine.UI;

public class ToggleDrugPrefab : MonoBehaviour
{
    public Drug drugAssigned;
    public Toggle toggle;
    private Image image;
    [SerializeField] RectTransform drugImage;

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        if (drugAssigned == null) return;

        image = drugImage.GetComponent<Image>();
        image.sprite = drugAssigned.icon;
    }

    public void RefreshLockState(int currentFactoryLevel)
    {
        if (drugAssigned == null) return;

        bool locked = drugAssigned.unlockLevel > currentFactoryLevel;

        if (toggle != null)
            toggle.interactable = !locked;

        if (image != null)
            image.color = locked ? new Color(1f, 1f, 1f, 0.4f) : Color.white;
    }
}