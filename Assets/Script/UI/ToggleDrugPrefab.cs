using UnityEngine;
using UnityEngine.UI;

public class ToggleDrugPrefab : MonoBehaviour
{
    public Drug drugAssigned;
    public Toggle toggle;
    Image image;
    [SerializeField] RectTransform drugImage;
    private void Start()
    {

        if(drugAssigned == null)
        {
            return;
        }
        image = drugImage.GetComponent<Image>();
        
        Debug.Log("Icon");
        image.sprite = drugAssigned.icon;

    }
}
