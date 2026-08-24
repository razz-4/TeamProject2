using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    Button[] buttons;
    Toggle[] toggleButtons;

    public int maxToggle;
    int currentToggleActive = 0;

    public GameObject Greenhouse;
    GreenHouse greenHouseScript;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }

    private void Update()
    {
    }

    private void Start()
    {
    }

    private void OnToggleChanged(int index, bool state)
    {
        if (state)
        {
            // Check if limit is reached
            if (currentToggleActive < maxToggle)
            {
                currentToggleActive++;
                greenHouseScript.ProductIngredients(ItemManager.Instance.ingredients[index]);
                Debug.Log("Added: " + ItemManager.Instance.ingredients[index].name);
            }
            else
            {
                //SetIsOnWithoutNotify sets the Toggle button true or false without triggering any Event.
                toggleButtons[index].SetIsOnWithoutNotify(false);
                Debug.Log("Toggle limit reached! Max allowed: " + maxToggle);
            }
        }
        else
        {
            // Only decrement if we actually had an active selection
            if (currentToggleActive > 0)
            {
                currentToggleActive--;
                greenHouseScript.CancelProductIngredients(ItemManager.Instance.ingredients[index]);
                Debug.Log("Removed: " + ItemManager.Instance.ingredients[index].name);
            }
        }
    }

    public void CheckGreenHouse(GameObject greenHouse)
    {
        if (greenHouse == null)
        {
            Debug.LogError("Passed Greenhouse GameObject is null!");
            return;
        }

        greenHouseScript = greenHouse.GetComponent<GreenHouse>();
        currentToggleActive = 0;

        toggleButtons = GetComponentsInChildren<Toggle>(true);
        Debug.Log($"Found {toggleButtons.Length} Toggle buttons in hierarchy.");

        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;

            toggleButtons[i].onValueChanged.RemoveAllListeners();
            toggleButtons[i].SetIsOnWithoutNotify(false);

            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                Debug.Log($"Button pressed on index: {index}, state: {state}");
                OnToggleChanged(index, state);
            });
        }
    }
}

/*
 *  public void CheckGreenHouse(GameObject greenHouse)
    {
        if (greenHouse == null)
        {
            Debug.LogError("Passed Greenhouse GameObject is null!");
            return;
        }

        greenHouseScript = greenHouse.GetComponent<GreenHouse>();
        currentToggleActive = 0;

        toggleButtons = GetComponentsInChildren<Toggle>(true);
        Debug.Log($"Found {toggleButtons.Length} Toggle buttons in hierarchy.");

        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;

            toggleButtons[i].onValueChanged.RemoveAllListeners();
            toggleButtons[i].SetIsOnWithoutNotify(false);

            toggleButtons[i].onValueChanged.AddListener((bool state) =>
            {
                Debug.Log($"Button pressed on index: {index}, state: {state}");
                OnToggleChanged(index, state);
            });
        }
    }
 * */