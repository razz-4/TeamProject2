using UnityEngine;
using UnityEngine.UI;

public class ToggleIngredientPrefab : MonoBehaviour
{
    public Ingredient ingredientAssigned;
    public Toggle toggle;
    Image image;
    [SerializeField] RectTransform ingredientImage;
    private void Start()
    {

        if (ingredientAssigned == null)
        {
            return;
        }
        image = ingredientImage.GetComponent<Image>();

        image.sprite = ingredientAssigned.icon;
    }
}