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
public sealed class SliderControl : BaseControl, IValueControl<float>, ITextControl
{
    private Slider? _slider;
    private TMP_Text? _labelText;
    private TMP_Text? _valueText;
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

        if (_slider != null)
        {
            _slider.onValueChanged.AddListener((Action<float>)OnSliderChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
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

    protected override void OnEnabledChanged(bool enabled)
    {
        if (_slider != null)
        {
            _slider.interactable = enabled;
        }
    }

    public override void Dispose()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.RemoveListener((Action<float>)OnSliderChanged);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
