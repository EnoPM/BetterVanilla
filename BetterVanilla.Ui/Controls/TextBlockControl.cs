using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A control that displays text with full TMP styling support.
/// </summary>
public sealed class TextBlockControl : BaseControl, ITextStyleControl
{
    private TextBlockComponent? _component;
    private readonly BindableProperty<string> _textProperty = new();

    #region Text Content

    public string Text
    {
        get => _component?.text.text ?? string.Empty;
        set
        {
            if (_component != null)
            {
                _component.text.text = value;
            }
            _textProperty.Value = value;
        }
    }

    #endregion

    #region Basic Styling

    public Color TextColor
    {
        get => _component?.text.color ?? Color.white;
        set
        {
            if (_component != null)
            {
                _component.text.color = value;
            }
        }
    }

    public float FontSize
    {
        get => _component?.text.fontSize ?? 14f;
        set
        {
            if (_component != null)
            {
                _component.text.fontSize = value;
            }
        }
    }

    public TextAlignmentOptions TextAlignment
    {
        get => _component?.text.alignment ?? TextAlignmentOptions.Left;
        set
        {
            if (_component != null)
            {
                _component.text.alignment = value;
            }
        }
    }

    public FontStyles FontStyle
    {
        get => _component?.text.fontStyle ?? FontStyles.Normal;
        set
        {
            if (_component != null)
            {
                _component.text.fontStyle = value;
            }
        }
    }

    #endregion

    #region Spacing

    public float CharacterSpacing
    {
        get => _component?.text.characterSpacing ?? 0f;
        set
        {
            if (_component != null)
            {
                _component.text.characterSpacing = value;
            }
        }
    }

    public float LineSpacing
    {
        get => _component?.text.lineSpacing ?? 0f;
        set
        {
            if (_component != null)
            {
                _component.text.lineSpacing = value;
            }
        }
    }

    public float WordSpacing
    {
        get => _component?.text.wordSpacing ?? 0f;
        set
        {
            if (_component != null)
            {
                _component.text.wordSpacing = value;
            }
        }
    }

    #endregion

    #region Text Behavior

    public bool WordWrapping
    {
        get => _component != null && _component.text.enableWordWrapping;
        set
        {
            if (_component != null)
            {
                _component.text.enableWordWrapping = value;
            }
        }
    }

    public TextOverflowModes TextOverflow
    {
        get => _component?.text.overflowMode ?? TextOverflowModes.Overflow;
        set
        {
            if (_component != null)
            {
                _component.text.overflowMode = value;
            }
        }
    }

    public bool RichText
    {
        get => _component != null && _component.text.richText;
        set
        {
            if (_component != null)
            {
                _component.text.richText = value;
            }
        }
    }

    #endregion

    #region Auto-Size

    public bool AutoSize
    {
        get => _component != null && _component.text.enableAutoSizing;
        set
        {
            if (_component != null)
            {
                _component.text.enableAutoSizing = value;
            }
        }
    }

    public float MinFontSize
    {
        get => _component?.text.fontSizeMin ?? 10f;
        set
        {
            if (_component != null)
            {
                _component.text.fontSizeMin = value;
            }
        }
    }

    public float MaxFontSize
    {
        get => _component?.text.fontSizeMax ?? 72f;
        set
        {
            if (_component != null)
            {
                _component.text.fontSizeMax = value;
            }
        }
    }

    public Vector4 TextMargin
    {
        get => _component?.text.margin ?? Vector4.zero;
        set
        {
            if (_component != null)
            {
                _component.text.margin = value;
            }
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<TextBlockComponent>();
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Text", _textProperty);
        _textProperty.ValueChanged += value =>
        {
            if (_component != null && value is string strValue)
            {
                _component.text.text = strValue;
            }
        };
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        if (_component != null)
        {
            _component.text.alpha = enabled ? 1f : 0.5f;
        }
    }
}
