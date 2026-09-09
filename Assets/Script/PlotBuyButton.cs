using UnityEngine;
using UnityEngine.UI;

public class PlotBuyButton : MonoBehaviour
{
    public int plotIndex; 

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            bool success = LandManager.Instance.BuyPlot(plotIndex);
            if (success)
                Debug.Log("buy success");
            else
                Debug.LogWarning("buy fail not enough gold");
        });
    }
}