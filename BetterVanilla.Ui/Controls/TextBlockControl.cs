using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A control that displays text.
/// </summary>
public sealed class TextBlockControl : BaseControl, ITextControl
{
    private TMP_Text? _textComponent;
    private readonly BindableProperty<string> _textProperty = new();

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

    protected override void Awake()
    {
        base.Awake();
        _textComponent = GetComponentInChildren<TMP_Text>();
    }

    protected override void RegisterBindableProperties()
    {
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
