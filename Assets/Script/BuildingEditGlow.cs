using UnityEngine;

public class BuildingEditGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _pulseSpeed = 3f;
    [SerializeField] private float _maxDarkenAmount = 0.8f;

    private MaterialPropertyBlock _propertyBlock;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private Color[] _originalColors;
    private int[] _colorProperties;

    private bool _isSelected = false;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();

        // Automatically find all renderers inside this building.
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        _originalColors = new Color[_renderers.Length];
        _colorProperties = new int[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer renderer = _renderers[i];

            if (renderer == null)
                continue;

            Material material = renderer.sharedMaterial;

            if (material == null)
                continue;

            // URP / HDRP
            if (material.HasProperty(BaseColor))
            {
                _colorProperties[i] = BaseColor;
                _originalColors[i] = material.GetColor(BaseColor);
            }
            // Built-in Standard shader
            else if (material.HasProperty(ColorID))
            {
                _colorProperties[i] = ColorID;
                _originalColors[i] = material.GetColor(ColorID);
            }
        }
    }

    private void Update()
    {
        if (!_isSelected)
            return;

        // Don't glow if building mode has ended.
        if (!DevelopersHub.ClashOfWhatecer.BuildingControl.IsBuildingMode)
        {
            StopGlow();
            return;
        }

        // Pulse from 0 -> 1 -> 0.
        float pulse = Mathf.PingPong(
            Time.time * _pulseSpeed,
            1f
        );

        ApplyGlow(pulse);
    }

    /// <summary>
    /// Turns this building's glow on.
    /// </summary>
    public void StartGlow()
    {
        _isSelected = true;
    }

    /// <summary>
    /// Turns this building's glow off.
    /// </summary>
    public void StopGlow()
    {
        _isSelected = false;
        ClearGlow();
    }

    private void ApplyGlow(float pulse)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer renderer = _renderers[i];

            if (renderer == null)
                continue;

            int propertyID = _colorProperties[i];

            if (propertyID == 0)
                continue;

            renderer.GetPropertyBlock(_propertyBlock);

            // Normal color -> black.
            float darkenAmount = pulse * _maxDarkenAmount;

            Color darkenedColor = Color.Lerp(
                _originalColors[i],
                UnityEngine.Color.black,
                darkenAmount
            );

            _propertyBlock.SetColor(
                propertyID,
                darkenedColor
            );

            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    private void ClearGlow()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null)
            {
                _renderers[i].SetPropertyBlock(null);
            }
        }
    }

    private void OnDisable()
    {
        ClearGlow();
        _isSelected = false;
    }
}
