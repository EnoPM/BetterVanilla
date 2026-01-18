using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A dropdown/combobox control for selecting from a list of options.
/// </summary>
public sealed class DropdownControl : BaseControl, IValueControl<int>, ILabelStyleControl
{
    private TMP_Dropdown? _dropdown;
    private TMP_Text? _labelText;
    private LabelStyleHelper? _labelStyle;
    private readonly BindableProperty<int> _selectedIndexProperty = new();
    private readonly BindableProperty<string> _textProperty = new();
    private OptionsBindableProperty? _optionsProperty;

    public event Action<int>? ValueChanged;
    public event Action<int>? SelectedIndexChanged;

    /// <summary>
    /// Gets or sets the selected index (0-based).
    /// </summary>
    public int Value
    {
        get => _dropdown?.value ?? -1;
        set
        {
            if (_dropdown != null && _dropdown.value != value)
            {
                _dropdown.value = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index (alias for Value).
    /// </summary>
    public int SelectedIndex
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Gets the currently selected option text.
    /// </summary>
    public string SelectedText
    {
        get
        {
            if (_dropdown == null || _dropdown.options.Count == 0 || _dropdown.value < 0)
                return string.Empty;
            return _dropdown.options[_dropdown.value].text;
        }
    }

    /// <summary>
    /// Gets or sets the label text displayed alongside the dropdown.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the list of options.
    /// </summary>
    public List<string> Options
    {
        get
        {
            var result = new List<string>();
            if (_dropdown != null)
            {
                foreach (var option in _dropdown.options)
                {
                    result.Add(option.text);
                }
            }
            return result;
        }
        set => SetOptions(value);
    }

    /// <summary>
    /// Sets the dropdown options from a list of strings.
    /// </summary>
    public void SetOptions(IEnumerable<string> options)
    {
        if (_dropdown == null)
            return;

        _dropdown.ClearOptions();
        var optionData = new Il2CppSystem.Collections.Generic.List<string>();
        foreach (var option in options)
        {
            optionData.Add(option);
        }
        
        _dropdown.AddOptions(optionData);
    }

    /// <summary>
    /// Sets the dropdown options from an enum type.
    /// </summary>
    public void SetOptionsFromEnum<TEnum>() where TEnum : Enum
    {
        SetOptions(Enum.GetNames(typeof(TEnum)));
    }

    /// <summary>
    /// Adds a single option to the dropdown.
    /// </summary>
    public void AddOption(string option)
    {
        _dropdown?.options.Add(new TMP_Dropdown.OptionData(option));
        _dropdown?.RefreshShownValue();
    }

    /// <summary>
    /// Clears all options from the dropdown.
    /// </summary>
    public void ClearOptions()
    {
        _dropdown?.ClearOptions();
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
            if (_dropdown != null)
            {
                _dropdown.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _dropdown = GetComponentInChildren<TMP_Dropdown>();

        // Find label text (usually the first TMP_Text that's not part of the dropdown)
        var texts = GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            // Skip the dropdown's caption text and item text
            if (_dropdown != null && (text == _dropdown.captionText || text == _dropdown.itemText))
                continue;
            _labelText = text;
            break;
        }

        _labelStyle = new LabelStyleHelper(_labelText);

        if (_dropdown != null)
        {
            _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        _optionsProperty = new OptionsBindableProperty(this);

        RegisterBindableProperty("SelectedIndex", _selectedIndexProperty);
        RegisterBindableProperty("Value", _selectedIndexProperty);
        RegisterBindableProperty("Text", _textProperty);
        RegisterBindableProperty("Options", _optionsProperty);

        _selectedIndexProperty.ValueChanged += value =>
        {
            if (_dropdown != null && value is int intValue)
            {
                _dropdown.value = intValue;
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

    private void OnDropdownValueChanged(int index)
    {
        _selectedIndexProperty.Value = index;
        ValueChanged?.Invoke(index);
        SelectedIndexChanged?.Invoke(index);
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_dropdown != null)
        {
            _dropdown.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_dropdown != null)
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
        ValueChanged = null;
        SelectedIndexChanged = null;
        base.Dispose();
    }

    /// <summary>
    /// Bindable property for dropdown options that accepts IEnumerable&lt;string&gt;.
    /// </summary>
    private sealed class OptionsBindableProperty : IBindableProperty
    {
        private readonly DropdownControl _control;
        private IEnumerable<string>? _options;

        public OptionsBindableProperty(DropdownControl control)
        {
            _control = control;
        }

        public object? GetValue() => _options;

        public void SetValue(object? value)
        {
            if (value is IEnumerable<string> options)
            {
                // Convert to list to avoid multiple enumeration
                var optionsList = options as IList<string> ?? new List<string>(options);
                _options = optionsList;
                _control.SetOptions(optionsList);
            }
        }

        public event Action<object?>? ValueChanged;
    }
}
