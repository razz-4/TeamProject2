using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientDescription : MonoBehaviour
{
    public Ingredient currentIngredient;
    public GreenHouse currentGreenHouse;
    public TextMeshProUGUI ingredientDescriptionText;

    [SerializeField] private Image ingredientImage;
    [SerializeField] private Toggle toggle;

    void Start()
    {
        toggle.onValueChanged.AddListener((bool state) =>
        {
            currentGreenHouse.CollectIngredient(currentIngredient);
            
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeDescription(Ingredient ingredient)
    {
        currentIngredient = ingredient;
        ingredientImage.sprite = ingredient.icon;
        ingredientDescriptionText.text = $"Name: {ingredient.name}\n\n" +
                                         $"Growth Time: {ingredient.perSecond} sec\n\n" +
                                         $"Yield: {ingredient.currentPerSecond}x";
    }



}
