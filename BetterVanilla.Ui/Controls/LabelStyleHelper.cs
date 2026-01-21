using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Helper class to apply text style properties to a TMP_Text component.
/// Used by controls that implement ILabelStyleControl.
/// </summary>
public sealed class LabelStyleHelper
{
    private readonly TMP_Text? _text;

    public LabelStyleHelper(TMP_Text? text)
    {
        _text = text;
    }

    public float FontSize
    {
        get => _text != null ? _text.fontSize : 14f;
        set
        {
            if (_text != null) _text.fontSize = value;
        }
    }

    public Color TextColor
    {
        get => _text != null ? _text.color : Color.white;
        set
        {
            if (_text != null) _text.color = value;
        }
    }

    public TextAlignmentOptions TextAlignment
    {
        get => _text != null ? _text.alignment : TextAlignmentOptions.Left;
        set
        {
            if (_text != null) _text.alignment = value;
        }
    }

    public FontStyles FontStyle
    {
        get => _text != null ? _text.fontStyle : FontStyles.Normal;
        set
        {
            if (_text != null) _text.fontStyle = value;
        }
    }

    public float CharacterSpacing
    {
        get => _text != null ? _text.characterSpacing : 0f;
        set
        {
            if (_text != null) _text.characterSpacing = value;
        }
    }

    public float LineSpacing
    {
        get => _text != null ? _text.lineSpacing : 0f;
        set
        {
            if (_text != null) _text.lineSpacing = value;
        }
    }

    public float WordSpacing
    {
        get => _text != null ? _text.wordSpacing : 0f;
        set
        {
            if (_text != null) _text.wordSpacing = value;
        }
    }

    public bool WordWrapping
    {
        get => _text != null && _text.enableWordWrapping;
        set
        {
            if (_text != null) _text.enableWordWrapping = value;
        }
    }

    public TextOverflowModes TextOverflow
    {
        get => _text != null ? _text.overflowMode : TextOverflowModes.Overflow;
        set
        {
            if (_text != null) _text.overflowMode = value;
        }
    }

    public bool RichText
    {
        get => _text != null && _text.richText;
        set
        {
            if (_text != null) _text.richText = value;
        }
    }

    public bool AutoSize
    {
        get => _text != null && _text.enableAutoSizing;
        set
        {
            if (_text != null) _text.enableAutoSizing = value;
        }
    }

    public float MinFontSize
    {
        get => _text != null ? _text.fontSizeMin : 10f;
        set
        {
            if (_text != null) _text.fontSizeMin = value;
        }
    }

    public float MaxFontSize
    {
        get => _text != null ? _text.fontSizeMax : 72f;
        set
        {
            if (_text != null) _text.fontSizeMax = value;
        }
    }

    public Vector4 TextMargin
    {
        get => _text != null ? _text.margin : Vector4.zero;
        set
        {
            if (_text != null) _text.margin = value;
        }
    }

    /// <summary>
    /// Parses a margin string into a Vector4.
    /// Formats: "10" (uniform), "10,5" (horizontal,vertical), "10,5,10,5" (left,top,right,bottom)
    /// </summary>
    public static Vector4 ParseMargin(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Vector4.zero;

        var parts = value.Split(',');
        return parts.Length switch
        {
            1 when float.TryParse(parts[0].Trim(), out var uniform) =>
                new Vector4(uniform, uniform, uniform, uniform),
            2 when float.TryParse(parts[0].Trim(), out var h) && float.TryParse(parts[1].Trim(), out var v) =>
                new Vector4(h, v, h, v),
            4 when float.TryParse(parts[0].Trim(), out var l) && float.TryParse(parts[1].Trim(), out var t) &&
                   float.TryParse(parts[2].Trim(), out var r) && float.TryParse(parts[3].Trim(), out var b) =>
                new Vector4(l, t, r, b),
            _ => Vector4.zero
        };
    }
}