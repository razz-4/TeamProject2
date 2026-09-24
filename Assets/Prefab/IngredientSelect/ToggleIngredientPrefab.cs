using UnityEngine;
using UnityEngine.UI;

public class ToggleIngredientPrefab : MonoBehaviour
{
    public Ingredient ingredientAssigned;
    public Toggle toggle;

    private Image image;

    [SerializeField] private RectTransform ingredientImage;

    private void Awake()
    {
        image = ingredientImage.GetComponent<Image>();

        if (toggle == null)
            toggle = GetComponent<Toggle>();
    }

    public void SetupIngredient(Ingredient ingredient)
    {
        ingredientAssigned = ingredient;

        if (image != null)
            image.sprite = ingredientAssigned.icon;

    }

    public void RefreshLockState(int currentGreenHouseLevel)
    {
        if (ingredientAssigned == null) return;

        bool locked = ingredientAssigned.unlockLevel > currentGreenHouseLevel;

        if (toggle != null)
            toggle.interactable = !locked;

        if (image != null)
            image.color = locked ? new Color(1f, 1f, 1f, 0.4f) : Color.white;
    }
}