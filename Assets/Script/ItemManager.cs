using UnityEngine;
using System.Linq;
using System.Threading;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    //Store ingredients and drugs
    public Ingredient[] ingredients;
    public Drug[] drugs;
    public int[] numbers;

    //Timer
    float maxTime = 1f;
    float timer = 1f;

    public void Awake()
    {
        //Check for duplicate and delete
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

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

    void GenerateIngredients()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            foreach (Ingredient ingredient in Instance.ingredients)
            {
                ingredient.quantity += ingredient.currentPerSecond;
            }
            timer = maxTime;
        }
    }

    private void Update()
    {
        //Suggestion add a timer
        GenerateIngredients();
    }
}