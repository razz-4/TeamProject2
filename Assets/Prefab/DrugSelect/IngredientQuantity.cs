using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientQuantity : MonoBehaviour
{
    [HideInInspector] public Ingredient ingredient;
    [HideInInspector] public int amountNeeded;
    private int amount = 0;
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
        quantityText.text = amount.ToString();


        
    }

    public void updateTextAmount(IngredientRequirement ingredient)
    {
        if(amount < ingredient.amount)
        {
            quantityText.text = ingredient.amount.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        amount = ingredient.quantity;
    }
}
