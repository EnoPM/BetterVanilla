using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A button control that can be clicked.
/// </summary>
public sealed class ButtonControl : BaseControl, ITextControl, IClickableControl
{
    private Button? _button;
    private TMP_Text? _textComponent;
    private readonly BindableProperty<string> _textProperty = new();

    public event Action? Clicked;

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

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_button != null)
            {
                _button.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponentInChildren<Button>();
        _textComponent = GetComponentInChildren<TMP_Text>();

        if (_button != null)
        {
            _button.onClick.AddListener((Action)OnClick);
        }
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

    private void OnClick()
    {
        if (IsEnabled)
        {
            Clicked?.Invoke();
        }
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        if (_button != null)
        {
            _button.interactable = enabled;
        }
    }

    public override void Dispose()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener((Action)OnClick);
        }
        Clicked = null;
        base.Dispose();
    }
}
