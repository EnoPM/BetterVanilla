using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A text input field control.
/// </summary>
public sealed class InputFieldControl : BaseControl, IValueControl<string>, ITextControl
{
    private TMP_InputField? _inputField;
    private readonly BindableProperty<string> _textProperty = new();

    public event Action<string>? ValueChanged;

    public string Value
    {
        get => _inputField != null ? _inputField.text : string.Empty;
        set
        {
            if (_inputField != null && _inputField.text != value)
            {
                _inputField.text = value;
            }
        }
    }

    public string Text
    {
        get => Value;
        set => Value = value;
    }

    public string Placeholder
    {
        get
        {
            if (_inputField?.placeholder is TMP_Text placeholder)
            {
                return placeholder.text;
            }
            return string.Empty;
        }
        set
        {
            if (_inputField?.placeholder is TMP_Text placeholder)
            {
                placeholder.text = value;
            }
        }
    }

    public TMP_InputField.ContentType ContentType
    {
        get => _inputField?.contentType ?? TMP_InputField.ContentType.Standard;
        set
        {
            if (_inputField != null)
            {
                _inputField.contentType = value;
            }
        }
    }

    public int CharacterLimit
    {
        get => _inputField?.characterLimit ?? 0;
        set
        {
            if (_inputField != null)
            {
                _inputField.characterLimit = value;
            }
        }
    }

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            if (_inputField != null)
            {
                _inputField.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _inputField = GetComponentInChildren<TMP_InputField>();

        if (_inputField != null)
        {
            _inputField.onValueChanged.AddListener(OnInputChanged);
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }
    }

    protected override void RegisterBindableProperties()
    {
        RegisterBindableProperty("Text", _textProperty);
        RegisterBindableProperty("Value", _textProperty);

        _textProperty.ValueChanged += value =>
        {
            if (_inputField != null && value is string strValue)
            {
                _inputField.text = strValue;
            }
        };
    }

    private void OnInputChanged(string value)
    {
        _textProperty.Value = value;
        ValueChanged?.Invoke(value);
    }

    private void OnEndEdit(string value)
    {
        // Can be used for validation or final value handling
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_inputField != null)
        {
            _inputField.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_inputField != null)
        {
            _inputField.onValueChanged.RemoveListener(OnInputChanged);
            _inputField.onEndEdit.RemoveListener(OnEndEdit);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
