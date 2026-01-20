using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A toggle/checkbox control.
/// </summary>
public sealed class ToggleControl : BaseControl, IValueControl<bool>, ILabelStyleControl
{
    private ToggleComponent? _component;
    private LabelStyleHelper? _labelStyle;
    private readonly BindableProperty<bool> _isOnProperty = new();
    private readonly BindableProperty<string> _textProperty = new();

    public event Action<bool>? ValueChanged;

    public bool Value
    {
        get => _component != null && _component.toggle.isOn;
        set
        {
            if (_component != null && _component.toggle.isOn != value)
            {
                _component.toggle.isOn = value;
            }
        }
    }

    public bool IsOn
    {
        get => Value;
        set => Value = value;
    }

    public string Text
    {
        get => _component?.label.text ?? string.Empty;
        set
        {
            if (_component != null)
            {
                _component.label.text = value;
            }
            _textProperty.Value = value;
        }
    }

    #region ILabelStyleControl

    public float LabelFontSize
    {
        get => _labelStyle?.FontSize ?? 14f;
        set { if (_labelStyle != null) _labelStyle.FontSize = value; }
    }

    public Color LabelTextColor
    {
        get => _labelStyle?.TextColor ?? Color.white;
        set { if (_labelStyle != null) _labelStyle.TextColor = value; }
    }

    public TextAlignmentOptions LabelTextAlignment
    {
        get => _labelStyle?.TextAlignment ?? TextAlignmentOptions.Left;
        set { if (_labelStyle != null) _labelStyle.TextAlignment = value; }
    }

    public FontStyles LabelFontStyle
    {
        get => _labelStyle?.FontStyle ?? FontStyles.Normal;
        set { if (_labelStyle != null) _labelStyle.FontStyle = value; }
    }

    public float LabelCharacterSpacing
    {
        get => _labelStyle?.CharacterSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.CharacterSpacing = value; }
    }

    public float LabelLineSpacing
    {
        get => _labelStyle?.LineSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.LineSpacing = value; }
    }

    public float LabelWordSpacing
    {
        get => _labelStyle?.WordSpacing ?? 0f;
        set { if (_labelStyle != null) _labelStyle.WordSpacing = value; }
    }

    public bool LabelWordWrapping
    {
        get => _labelStyle?.WordWrapping ?? false;
        set { if (_labelStyle != null) _labelStyle.WordWrapping = value; }
    }

    public TextOverflowModes LabelTextOverflow
    {
        get => _labelStyle?.TextOverflow ?? TextOverflowModes.Overflow;
        set { if (_labelStyle != null) _labelStyle.TextOverflow = value; }
    }

    public bool LabelRichText
    {
        get => _labelStyle?.RichText ?? true;
        set { if (_labelStyle != null) _labelStyle.RichText = value; }
    }

    public bool LabelAutoSize
    {
        get => _labelStyle?.AutoSize ?? false;
        set { if (_labelStyle != null) _labelStyle.AutoSize = value; }
    }

    public float LabelMinFontSize
    {
        get => _labelStyle?.MinFontSize ?? 10f;
        set { if (_labelStyle != null) _labelStyle.MinFontSize = value; }
    }

    public float LabelMaxFontSize
    {
        get => _labelStyle?.MaxFontSize ?? 72f;
        set { if (_labelStyle != null) _labelStyle.MaxFontSize = value; }
    }

    public Vector4 LabelTextMargin
    {
        get => _labelStyle?.TextMargin ?? Vector4.zero;
        set { if (_labelStyle != null) _labelStyle.TextMargin = value; }
    }

    #endregion

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_component != null)
            {
                _component.toggle.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<ToggleComponent>();
        _labelStyle = new LabelStyleHelper(_component?.label);

        if (_component != null)
        {
            _component.toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("IsOn", _isOnProperty);
        RegisterBindableProperty("Value", _isOnProperty);
        RegisterBindableProperty("Text", _textProperty);

        _isOnProperty.ValueChanged += value =>
        {
            if (_component != null && value is bool boolValue)
            {
                _component.toggle.isOn = boolValue;
            }
        };

        _textProperty.ValueChanged += value =>
        {
            if (_component != null && value is string strValue)
            {
                _component.label.text = strValue;
            }
        };
    }

    private void OnToggleChanged(bool isOn)
    {
        _isOnProperty.Value = isOn;
        ValueChanged?.Invoke(isOn);
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_component != null)
        {
            _component.toggle.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_component != null)
        {
            _component.toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
