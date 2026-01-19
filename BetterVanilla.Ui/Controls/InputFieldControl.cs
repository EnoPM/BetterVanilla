using System;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A text input field control.
/// </summary>
public sealed class InputFieldControl : BaseControl, IValueControl<string>, ITextControl, ILabelStyleControl, IPlaceholderStyleControl
{
    private InputFieldComponent? _component;
    private LabelStyleHelper? _labelStyle;
    private LabelStyleHelper? _placeholderStyle;
    private readonly BindableProperty<string> _textProperty = new();

    public event Action<string>? ValueChanged;

    public string Value
    {
        get => _component != null ? _component.inputField.text : string.Empty;
        set
        {
            if (_component != null && _component.inputField.text != value)
            {
                _component.inputField.text = value;
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
        get => _component?.placeholder.text ?? string.Empty;
        set
        {
            if (_component != null)
            {
                _component.placeholder.text = value;
            }
        }
    }

    public TMP_InputField.ContentType ContentType
    {
        get => _component?.inputField.contentType ?? TMP_InputField.ContentType.Standard;
        set
        {
            if (_component != null)
            {
                _component.inputField.contentType = value;
            }
        }
    }

    public int CharacterLimit
    {
        get => _component?.inputField.characterLimit ?? 0;
        set
        {
            if (_component != null)
            {
                _component.inputField.characterLimit = value;
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
                _component.inputField.interactable = value;
            }
        }
    }

    #region ILabelStyleControl (Text Style)

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

    #region IPlaceholderStyleControl

    public float PlaceholderFontSize
    {
        get => _placeholderStyle?.FontSize ?? 14f;
        set { if (_placeholderStyle != null) _placeholderStyle.FontSize = value; }
    }

    public Color PlaceholderTextColor
    {
        get => _placeholderStyle?.TextColor ?? new Color(0.5f, 0.5f, 0.5f, 0.5f);
        set { if (_placeholderStyle != null) _placeholderStyle.TextColor = value; }
    }

    public TextAlignmentOptions PlaceholderTextAlignment
    {
        get => _placeholderStyle?.TextAlignment ?? TextAlignmentOptions.Left;
        set { if (_placeholderStyle != null) _placeholderStyle.TextAlignment = value; }
    }

    public FontStyles PlaceholderFontStyle
    {
        get => _placeholderStyle?.FontStyle ?? FontStyles.Italic;
        set { if (_placeholderStyle != null) _placeholderStyle.FontStyle = value; }
    }

    public float PlaceholderCharacterSpacing
    {
        get => _placeholderStyle?.CharacterSpacing ?? 0f;
        set { if (_placeholderStyle != null) _placeholderStyle.CharacterSpacing = value; }
    }

    public float PlaceholderLineSpacing
    {
        get => _placeholderStyle?.LineSpacing ?? 0f;
        set { if (_placeholderStyle != null) _placeholderStyle.LineSpacing = value; }
    }

    public float PlaceholderWordSpacing
    {
        get => _placeholderStyle?.WordSpacing ?? 0f;
        set { if (_placeholderStyle != null) _placeholderStyle.WordSpacing = value; }
    }

    public bool PlaceholderWordWrapping
    {
        get => _placeholderStyle?.WordWrapping ?? false;
        set { if (_placeholderStyle != null) _placeholderStyle.WordWrapping = value; }
    }

    public TextOverflowModes PlaceholderTextOverflow
    {
        get => _placeholderStyle?.TextOverflow ?? TextOverflowModes.Overflow;
        set { if (_placeholderStyle != null) _placeholderStyle.TextOverflow = value; }
    }

    public bool PlaceholderRichText
    {
        get => _placeholderStyle?.RichText ?? true;
        set { if (_placeholderStyle != null) _placeholderStyle.RichText = value; }
    }

    public bool PlaceholderAutoSize
    {
        get => _placeholderStyle?.AutoSize ?? false;
        set { if (_placeholderStyle != null) _placeholderStyle.AutoSize = value; }
    }

    public float PlaceholderMinFontSize
    {
        get => _placeholderStyle?.MinFontSize ?? 10f;
        set { if (_placeholderStyle != null) _placeholderStyle.MinFontSize = value; }
    }

    public float PlaceholderMaxFontSize
    {
        get => _placeholderStyle?.MaxFontSize ?? 72f;
        set { if (_placeholderStyle != null) _placeholderStyle.MaxFontSize = value; }
    }

    public Vector4 PlaceholderTextMargin
    {
        get => _placeholderStyle?.TextMargin ?? Vector4.zero;
        set { if (_placeholderStyle != null) _placeholderStyle.TextMargin = value; }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<InputFieldComponent>();

        if (_component != null)
        {
            _labelStyle = new LabelStyleHelper(_component.text);
            _placeholderStyle = new LabelStyleHelper(_component.placeholder);

            _component.inputField.onValueChanged.AddListener(OnInputChanged);
            _component.inputField.onEndEdit.AddListener(OnEndEdit);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        RegisterBindableProperty("Text", _textProperty);
        RegisterBindableProperty("Value", _textProperty);

        _textProperty.ValueChanged += value =>
        {
            if (_component != null && value is string strValue)
            {
                _component.inputField.text = strValue;
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
        if (_component != null)
        {
            _component.inputField.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_component != null)
        {
            _component.inputField.onValueChanged.RemoveListener(OnInputChanged);
            _component.inputField.onEndEdit.RemoveListener(OnEndEdit);
        }
        ValueChanged = null;
        base.Dispose();
    }
}
