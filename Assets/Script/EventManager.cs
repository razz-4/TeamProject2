using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public UnityEvent<Ingredient, int> InventoryUpdateEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void InventoryUpdated(Ingredient ingredient, int count)
    {
        InventoryUpdateEvent.Invoke(ingredient, count);
    }

}
