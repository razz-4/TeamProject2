using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public Ingredient[] ingredients;
    public void Awake()
    {
        Instance = this;

        ingredients = Utils.GetAllInstances<Ingredient>();
        ingredients = ingredients.OrderBy(p => p.id).ToArray();
    }
}
