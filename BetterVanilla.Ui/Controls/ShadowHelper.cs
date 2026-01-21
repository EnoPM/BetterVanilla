using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Helper class to manage Shadow component properties.
/// Used by controls that implement IShadowControl.
/// </summary>
public sealed class ShadowHelper
{
    private readonly Shadow? _shadow;

    public ShadowHelper(Shadow? shadow)
    {
        _shadow = shadow;
    }

    public bool Enabled
    {
        get => _shadow != null && _shadow.enabled;
        set
        {
            if (_shadow != null)
            {
                _shadow.enabled = value;
            }
        }
    }

    public Color Color
    {
        get => _shadow?.effectColor ?? new Color(0, 0, 0, 0.5f);
        set
        {
            if (_shadow != null)
            {
                _shadow.effectColor = value;
            }
        }
    }

    public Vector2 Distance
    {
        get => _shadow?.effectDistance ?? new Vector2(1, -1);
        set
        {
            if (_shadow != null)
            {
                _shadow.effectDistance = value;
            }
        }
    }

    public bool UseGraphicAlpha
    {
        get => _shadow != null && _shadow.useGraphicAlpha;
        set
        {
            if (_shadow != null)
            {
                _shadow.useGraphicAlpha = value;
            }
        }
    }

    /// <summary>
    /// Parses a shadow distance string into a Vector2.
    /// Formats: "5" (uniform), "5,-5" (x,y)
    /// </summary>
    public static Vector2 ParseDistance(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Vector2(1, -1);
        }

        var parts = value.Split(',');
        return parts.Length switch
        {
            1 when float.TryParse(parts[0].Trim(), out var uniform) =>
                new Vector2(uniform, -uniform),
            2 when float.TryParse(parts[0].Trim(), out var x) && float.TryParse(parts[1].Trim(), out var y) =>
                new Vector2(x, y),
            _ => new Vector2(1, -1)
        };
    }
}
