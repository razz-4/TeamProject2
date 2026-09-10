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
    [SerializeField] TextMeshProUGUI plantText;



    void Start()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnPlantToggleChanged);
    }

    private void OnPlantToggleChanged(bool state)
    {
        if (currentGreenHouse == null || currentIngredient == null) return;

        if (state)
        {
            currentGreenHouse.SetIngredient(currentIngredient);
            currentGreenHouse.StartTimer();
            currentGreenHouse.CollectLastIngredient();
            UIBuildingManager.Instance.HideUI();
        }
        else
        {
            currentGreenHouse.CollectLastIngredient();
            currentGreenHouse.GetTimer()?.StopTimer();
            currentGreenHouse.SetIngredient(null);
        }
    }

    // Pass the active GreenHouse instance when updating description UI
    public void ChangeDescription(Ingredient ingredient, GreenHouse greenhouse)
    {
        currentIngredient = ingredient;
        currentGreenHouse = greenhouse;

        ingredientImage.sprite = ingredient.icon;
        ingredientDescriptionText.text = $"Name: {ingredient.name}\n\n" +
                                         $"Growth Time: {ingredient.perSecond} sec\n\n" +
                                         $"Yield: {ingredient.currentPerSecond}x";

        // Check if THIS specific building is currently growing this ingredient
        bool isPlanted = (currentGreenHouse != null && currentGreenHouse.currentIngredient == currentIngredient);

        // Update toggle UI without triggering onValueChanged
        UpdateButtonText(isPlanted);
        toggle.SetIsOnWithoutNotify(isPlanted);

    }

    private void UpdateButtonText(bool isPlanted)
    {
        if (plantText != null)
        {
            plantText.text = isPlanted ? "Cancel" : "Plant";
        }
    }
}
