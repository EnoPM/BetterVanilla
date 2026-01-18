using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A slider control for numeric values.
/// </summary>
public sealed class SliderControl : BaseControl, IValueControl<float>, ILabelStyleControl
{
    private Slider? _slider;
    private TMP_Text? _labelText;
    private TMP_Text? _valueText;
    private LabelStyleHelper? _labelStyle;
    private readonly BindableProperty<float> _valueProperty = new();
    private readonly BindableProperty<string> _textProperty = new();

    public event Action<float>? ValueChanged;

    public float Value
    {
        get => _slider != null ? _slider.value : 0f;
        set
        {
            if (_slider != null && !Mathf.Approximately(_slider.value, value))
            {
                _slider.value = value;
            }
        }
    }

    public float MinValue
    {
        get => _slider != null ? _slider.minValue : 0f;
        set
        {
            if (_slider != null)
            {
                _slider.minValue = value;
            }
        }
    }

    public float MaxValue
    {
        get => _slider != null ? _slider.maxValue : 1f;
        set
        {
            if (_slider != null)
            {
                _slider.maxValue = value;
            }
        }
    }

    public bool WholeNumbers
    {
        get => _slider != null && _slider.wholeNumbers;
        set
        {
            if (_slider != null)
            {
                _slider.wholeNumbers = value;
            }
        }
    }

    public string Text
    {
        get => _labelText != null ? _labelText.text : string.Empty;
        set
        {
            if (_labelText != null)
            {
                _labelText.text = value;
            }
            _textProperty.Value = value;
        }
    }

    public string? ValueFormat { get; set; } = "{0:F1}";

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
            if (_slider != null)
            {
                _slider.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _slider = GetComponentInChildren<Slider>();

        // Find label and value texts
        var texts = GetComponentsInChildren<TMP_Text>();
        if (texts.Length >= 1)
            _labelText = texts[0];
        if (texts.Length >= 2)
            _valueText = texts[1];

        _labelStyle = new LabelStyleHelper(_labelText);

        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Value", _valueProperty);
        RegisterBindableProperty("Text", _textProperty);

        _valueProperty.ValueChanged += value =>
        {
            if (_slider != null && value is float floatValue)
            {
                _slider.value = floatValue;
            }
        };

        _textProperty.ValueChanged += value =>
        {
            if (_labelText != null && value is string strValue)
            {
                _labelText.text = strValue;
            }
        };
    }

    private void OnSliderChanged(float value)
    {
        _valueProperty.Value = value;
        UpdateValueText();
        ValueChanged?.Invoke(value);
    }

    private void UpdateValueText()
    {
        if (_valueText != null && !string.IsNullOrEmpty(ValueFormat))
        {
            _valueText.text = string.Format(ValueFormat, Value);
        }
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_slider != null)
        {
            _slider.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
