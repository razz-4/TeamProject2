using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientQuantity : MonoBehaviour
{
    [HideInInspector] public Ingredient ingredient;
    [SerializeField] public TextMeshProUGUI quantityText;
    [SerializeField] Image ingredientImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(ingredient == null)
        {
            Debug.LogError("Ingredient is null");
            return;
        }
        ingredientImage.sprite = ingredient.icon;


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
