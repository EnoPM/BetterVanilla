using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A control that displays text with full TMP styling support.
/// </summary>
public sealed class TextBlockControl : BaseControl, ITextStyleControl
{
    private TMP_Text? _textComponent;
    private readonly BindableProperty<string> _textProperty = new();

    #region Text Content

    public string Text
    {
        get => _textComponent != null ? _textComponent.text : string.Empty;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.text = value;
            }
            _textProperty.Value = value;
        }
    }

    #endregion

    #region Basic Styling

    public Color TextColor
    {
        get => _textComponent != null ? _textComponent.color : Color.white;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.color = value;
            }
        }
    }

    public float FontSize
    {
        get => _textComponent != null ? _textComponent.fontSize : 14f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.fontSize = value;
            }
        }
    }

    public TextAlignmentOptions TextAlignment
    {
        get => _textComponent != null ? _textComponent.alignment : TextAlignmentOptions.Left;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.alignment = value;
            }
        }
    }

    public FontStyles FontStyle
    {
        get => _textComponent != null ? _textComponent.fontStyle : FontStyles.Normal;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.fontStyle = value;
            }
        }
    }

    #endregion

    #region Spacing

    public float CharacterSpacing
    {
        get => _textComponent != null ? _textComponent.characterSpacing : 0f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.characterSpacing = value;
            }
        }
    }

    public float LineSpacing
    {
        get => _textComponent != null ? _textComponent.lineSpacing : 0f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.lineSpacing = value;
            }
        }
    }

    public float WordSpacing
    {
        get => _textComponent != null ? _textComponent.wordSpacing : 0f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.wordSpacing = value;
            }
        }
    }

    #endregion

    #region Text Behavior

    public bool WordWrapping
    {
        get => _textComponent != null && _textComponent.enableWordWrapping;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.enableWordWrapping = value;
            }
        }
    }

    public TextOverflowModes TextOverflow
    {
        get => _textComponent != null ? _textComponent.overflowMode : TextOverflowModes.Overflow;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.overflowMode = value;
            }
        }
    }

    public bool RichText
    {
        get => _textComponent != null && _textComponent.richText;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.richText = value;
            }
        }
    }

    #endregion

    #region Auto-Size

    public bool AutoSize
    {
        get => _textComponent != null && _textComponent.enableAutoSizing;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.enableAutoSizing = value;
            }
        }
    }

    public float MinFontSize
    {
        get => _textComponent != null ? _textComponent.fontSizeMin : 10f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.fontSizeMin = value;
            }
        }
    }

    public float MaxFontSize
    {
        get => _textComponent != null ? _textComponent.fontSizeMax : 72f;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.fontSizeMax = value;
            }
        }
    }

    public Vector4 TextMargin
    {
        get => _textComponent != null ? _textComponent.margin : Vector4.zero;
        set
        {
            if (_textComponent != null)
            {
                _textComponent.margin = value;
            }
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _textComponent = GetComponentInChildren<TMP_Text>();
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Text", _textProperty);
        _textProperty.ValueChanged += value =>
        {
            if (_textComponent != null && value is string strValue)
            {
                _textComponent.text = strValue;
            }
        };
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        if (_textComponent != null)
        {
            _textComponent.alpha = enabled ? 1f : 0.5f;
        }
    }
}
