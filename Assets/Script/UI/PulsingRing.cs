using UnityEngine;
using UnityEngine.UI;

public class PulsingRing : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseDuration = 0.8f;
    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float endScale = 1.5f;

    [Header("Fade Settings")]
    [SerializeField] private float startAlpha = 1.0f;
    [SerializeField] private float endAlpha = 0.0f;

    private Vector3 baseScale;
    private Image ringImage;
    private float timer;

    private void Awake()
    {
        baseScale = transform.localScale;
        ringImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Increment timer and wrap around when reaching pulseDuration
        timer += Time.deltaTime;
        if (timer >= pulseDuration)
        {
            timer %= pulseDuration;
        }

        // Linear normalized progress (0.0 to 1.0)
        float progress = timer / pulseDuration;

        // Scale outward from startScale to endScale
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        transform.localScale = baseScale * currentScale;

        // Fade out from startAlpha to endAlpha
        float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, progress);
        SetAlpha(currentAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (ringImage != null)
        {
            Color c = ringImage.color;
            c.a = alpha;
            ringImage.color = c;
        }

    }
}
