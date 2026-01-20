using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A slider control for numeric values.
/// </summary>
public sealed class SliderControl : BaseControl, IValueControl<float>
{
    private SliderComponent? _component;
    private readonly BindableProperty<float> _valueProperty = new();

    public event Action<float>? ValueChanged;

    public float Value
    {
        get => _component?.slider.value ?? 0f;
        set
        {
            if (_component != null && !Mathf.Approximately(_component.slider.value, value))
            {
                _component.slider.value = value;
            }
        }
    }

    public float MinValue
    {
        get => _component?.slider.minValue ?? 0f;
        set
        {
            if (_component != null)
            {
                _component.slider.minValue = value;
            }
        }
    }

    public float MaxValue
    {
        get => _component?.slider.maxValue ?? 1f;
        set
        {
            if (_component != null)
            {
                _component.slider.maxValue = value;
            }
        }
    }

    public bool WholeNumbers
    {
        get => _component != null && _component.slider.wholeNumbers;
        set
        {
            if (_component != null)
            {
                _component.slider.wholeNumbers = value;
            }
        }
    }

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_component != null)
            {
                _component.slider.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<SliderComponent>();

        if (_component != null)
        {
            _component.slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Value", _valueProperty);

        _valueProperty.ValueChanged += value =>
        {
            if (_component != null && value is float floatValue)
            {
                _component.slider.value = floatValue;
            }
        };
    }

    private void OnSliderChanged(float value)
    {
        _valueProperty.Value = value;
        ValueChanged?.Invoke(value);
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_component != null)
        {
            _component.slider.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_component != null)
        {
            _component.slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
