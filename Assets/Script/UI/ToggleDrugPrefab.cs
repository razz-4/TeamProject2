using UnityEngine;
using UnityEngine.UI;

public class ToggleDrugPrefab : MonoBehaviour
{
    public Drug drugAssigned;
    public Toggle toggle;
    Image checkMarkImage;
    [SerializeField] Transform checkMarkTransform;
    private void Start()
    {
        checkMarkImage = checkMarkTransform.GetComponent<Image>();
        if (checkMarkImage == null)
        {
            Debug.Log("Return");
            return;
        }

        Debug.Log("Icon");
        checkMarkImage.sprite = drugAssigned.Icon;

    }
}
