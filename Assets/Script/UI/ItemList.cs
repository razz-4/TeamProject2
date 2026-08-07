using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    Button[] buttons;
    public GameObject Greenhouse;
    GreenHouse greenHouseScript;
    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }

    private void Update()
    {
        CheckGreenHouse();
    }

    private void Start()
    {

        for (int i = 0; i < buttons.Length; i++)
        {
            // CRITICAL: Capture the current index in a local variable!
            int index = i;
            buttons[i].onClick.AddListener(() => 
            { 
                greenHouseScript.ProductIngredients(ItemManager.Instance.ingredients[index]);
                Debug.Log(ItemManager.Instance.ingredients[index].name);
            });  

        }
    }

    void CheckGreenHouse()
    {
        //Safety check
        if (Greenhouse != null)
        {
            greenHouseScript = Greenhouse.GetComponent<GreenHouse>();
        }
        else
        {
            return;
        }
    }
}
