using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public Ingredient[] ingredients;

    public Drug[] drugs;
    public void Awake()
    {
        Instance = this;

        ingredients = Utils.GetAllIngredientInstances<Ingredient>();
        ingredients = ingredients.OrderBy(p => p.id).ToArray();

        drugs = Utils.GetAllDrugInstances<Drug>();
        drugs = drugs.OrderBy(p => p.id).ToArray();
    }


    public bool synthesize(Drug drug)
    {
        foreach (IngredientRequirement req in drug.recipe)
        {
            Ingredient stock = ingredients.FirstOrDefault(i => i.id == req.ingredient.id);
            int needed = req.amount * drug.perSecond;

            if (stock == null || stock.quantity < needed)
            {
                Debug.Log($"Not enough {req.ingredient.name} to synthesize {drug.name}");
                return false;
            }

        }

        foreach (IngredientRequirement req in drug.recipe)
        {
            Ingredient stock = ingredients.First(i => i.id == req.ingredient.id);
            stock.quantity -= req.amount * drug.perSecond;
        }

        drug.quantity += drug.perSecond;

        return true;
    }
}


