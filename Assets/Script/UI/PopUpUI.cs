using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText; // Or 'public Text quantityText;'
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float floatSpeed = 1.0f;

    public void Setup(Sprite icon, int quantity)
    {
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
        }

        if (quantityText != null)
        {
            quantityText.text = $"+{quantity}";
        }

        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Slowly float upward over time
        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
    }
}
