using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A toggle/checkbox control.
/// </summary>
public sealed class ToggleControl : BaseControl, IValueControl<bool>, ITextControl
{
    private Toggle? _toggle;
    private TMP_Text? _labelText;
    private readonly BindableProperty<bool> _isOnProperty = new();
    private readonly BindableProperty<string> _textProperty = new();

    public event Action<bool>? ValueChanged;

    public bool Value
    {
        get => _toggle != null && _toggle.isOn;
        set
        {
            if (_toggle != null && _toggle.isOn != value)
            {
                _toggle.isOn = value;
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

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_toggle != null)
            {
                _toggle.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _toggle = GetComponentInChildren<Toggle>();
        _labelText = GetComponentInChildren<TMP_Text>();

        if (_toggle != null)
        {
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        RegisterBindableProperty("IsOn", _isOnProperty);
        RegisterBindableProperty("Value", _isOnProperty);
        RegisterBindableProperty("Text", _textProperty);

        _isOnProperty.ValueChanged += value =>
        {
            if (_toggle != null && value is bool boolValue)
            {
                _toggle.isOn = boolValue;
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

    private void OnToggleChanged(bool isOn)
    {
        _isOnProperty.Value = isOn;
        ValueChanged?.Invoke(isOn);
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_toggle != null)
        {
            _toggle.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_toggle != null)
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
