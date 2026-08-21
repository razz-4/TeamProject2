using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    Button[] buttons;
    Toggle[] toggleButtons;
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
        //Get all toggle buttons first
        toggleButtons = GetComponentsInChildren<Toggle>(true);

        //For Toggle Button
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            Debug.Log(toggleButtons[i].name);

            int index = i;
            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                if (state)
                {
                    greenHouseScript.ProductIngredients(ItemManager.Instance.ingredients[index]);
                    Debug.Log(ItemManager.Instance.ingredients[index].name);
                }
                else
                {
                    greenHouseScript.CancelProductIngredients(ItemManager.Instance.ingredients[index]);
                    Debug.Log(ItemManager.Instance.ingredients[index].name);
                }
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
