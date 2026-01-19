using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// Helper class to apply color transition properties to a Button component.
/// Used by controls that implement IButtonColorsControl.
/// </summary>
public sealed class ButtonColorsHelper
{
    private readonly Button? _button;

    public ButtonColorsHelper(Button? button)
    {
        _button = button;
    }

    public Color NormalColor
    {
        get => _button?.colors.normalColor ?? Color.white;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.normalColor = value;
            _button.colors = colors;
        }
    }

    public Color HighlightedColor
    {
        get => _button?.colors.highlightedColor ?? Color.white;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.highlightedColor = value;
            _button.colors = colors;
        }
    }

    public Color PressedColor
    {
        get => _button?.colors.pressedColor ?? Color.gray;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.pressedColor = value;
            _button.colors = colors;
        }
    }

    public Color SelectedColor
    {
        get => _button?.colors.selectedColor ?? Color.white;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.selectedColor = value;
            _button.colors = colors;
        }
    }

    public Color DisabledColor
    {
        get => _button?.colors.disabledColor ?? new Color(0.5f, 0.5f, 0.5f, 0.5f);
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.disabledColor = value;
            _button.colors = colors;
        }
    }

    public float ColorMultiplier
    {
        get => _button?.colors.colorMultiplier ?? 1f;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.colorMultiplier = value;
            _button.colors = colors;
        }
    }

    public float FadeDuration
    {
        get => _button?.colors.fadeDuration ?? 0.1f;
        set
        {
            if (_button == null) return;
            var colors = _button.colors;
            colors.fadeDuration = value;
            _button.colors = colors;
        }
    }
}
