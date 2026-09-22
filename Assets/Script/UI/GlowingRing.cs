using UnityEngine;

public class GlowingRing : MonoBehaviour
{
    private CanvasGroup canvaGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvaGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        HideRing();
    }
    public void ShowRing()
    {
        canvaGroup.alpha = 1;
    }

    public void HideRing()
    {
        canvaGroup.alpha = 0;
    }

    public void SetPosition(Vector3 position)
    {
        rectTransform.anchoredPosition = position;
    }
}
