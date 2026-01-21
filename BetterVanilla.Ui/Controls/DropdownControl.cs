using System;
using System.Collections.Generic;
using BetterVanilla.Ui.Extensions;
using BetterVanilla.Ui.Binding;
using BetterVanilla.Ui.Components;
using BetterVanilla.Ui.Core;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Controls;

/// <summary>
/// A dropdown/combobox control for selecting from a list of options.
/// </summary>
public sealed class DropdownControl : BaseControl, IValueControl<int>, ILabelStyleControl
{
    private DropdownComponent? _component;
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
        get => _component?.dropdown.value ?? -1;
        set
        {
            if (_component != null && _component.dropdown.value != value)
            {
                _component.dropdown.value = value;
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
            if (_component == null || _component.dropdown.options.Count == 0 || _component.dropdown.value < 0)
                return string.Empty;
            return _component.dropdown.options[_component.dropdown.value].text;
        }
    }

    /// <summary>
    /// Gets or sets the label text displayed alongside the dropdown.
    /// </summary>
    public string Text
    {
        get => _component != null ? _component.label.text : string.Empty;
        set
        {
            if (_component != null)
            {
                _component.label.text = value;
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
            if (_component != null)
            {
                foreach (var option in _component.dropdown.options)
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
        if (_component == null) return;

        _component.dropdown.ClearOptions();
        var optionData = new Il2CppSystem.Collections.Generic.List<string>();
        foreach (var option in options)
        {
            optionData.Add(option);
        }

        _component.dropdown.AddOptions(optionData);
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
        _component?.dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        _component?.dropdown.RefreshShownValue();
    }

    /// <summary>
    /// Clears all options from the dropdown.
    /// </summary>
    public void ClearOptions()
    {
        _component?.dropdown.ClearOptions();
    }

    #region ILabelStyleControl

    public float LabelFontSize
    {
        get => _labelStyle?.FontSize ?? 14f;
        set => _labelStyle?.FontSize = value;
    }

    public Color LabelTextColor
    {
        get => _labelStyle?.TextColor ?? Color.white;
        set => _labelStyle?.TextColor = value;
    }

    public TextAlignmentOptions LabelTextAlignment
    {
        get => _labelStyle?.TextAlignment ?? TextAlignmentOptions.Left;
        set => _labelStyle?.TextAlignment = value;
    }

    public FontStyles LabelFontStyle
    {
        get => _labelStyle?.FontStyle ?? FontStyles.Normal;
        set => _labelStyle?.FontStyle = value;
    }

    public float LabelCharacterSpacing
    {
        get => _labelStyle?.CharacterSpacing ?? 0f;
        set => _labelStyle?.CharacterSpacing = value;
    }

    public float LabelLineSpacing
    {
        get => _labelStyle?.LineSpacing ?? 0f;
        set => _labelStyle?.LineSpacing = value;
    }

    public float LabelWordSpacing
    {
        get => _labelStyle?.WordSpacing ?? 0f;
        set => _labelStyle?.WordSpacing = value;
    }

    public bool LabelWordWrapping
    {
        get => _labelStyle?.WordWrapping ?? false;
        set => _labelStyle?.WordWrapping = value;
    }

    public TextOverflowModes LabelTextOverflow
    {
        get => _labelStyle?.TextOverflow ?? TextOverflowModes.Overflow;
        set => _labelStyle?.TextOverflow = value;
    }

    public bool LabelRichText
    {
        get => _labelStyle?.RichText ?? true;
        set => _labelStyle?.RichText = value;
    }

    public bool LabelAutoSize
    {
        get => _labelStyle?.AutoSize ?? false;
        set => _labelStyle?.AutoSize = value;
    }

    public float LabelMinFontSize
    {
        get => _labelStyle?.MinFontSize ?? 10f;
        set => _labelStyle?.MinFontSize = value;
    }

    public float LabelMaxFontSize
    {
        get => _labelStyle?.MaxFontSize ?? 72f;
        set => _labelStyle?.MaxFontSize = value;
    }

    public Vector4 LabelTextMargin
    {
        get => _labelStyle?.TextMargin ?? Vector4.zero;
        set => _labelStyle?.TextMargin = value;
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
                _component.dropdown.interactable = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _component = GetComponent<DropdownComponent>();
        _labelStyle = new LabelStyleHelper(_component?.label);

        if (_component != null)
        {
            _component.dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    protected override void RegisterBindableProperties()
    {
        base.RegisterBindableProperties();

        _optionsProperty = new OptionsBindableProperty(this);

        RegisterBindableProperty(nameof(SelectedIndex), _selectedIndexProperty);
        RegisterBindableProperty(nameof(Value), _selectedIndexProperty);
        RegisterBindableProperty(nameof(Text), _textProperty);
        RegisterBindableProperty(nameof(Options), _optionsProperty);

        _selectedIndexProperty.ValueChanged += value =>
        {
            if (_component != null && value is int intValue)
            {
                _component.dropdown.value = intValue;
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

    private void OnDropdownValueChanged(int index)
    {
        _selectedIndexProperty.Value = index;
        ValueChanged?.Invoke(index);
        SelectedIndexChanged?.Invoke(index);
    }

    protected override void OnEnabledChanged(bool state)
    {
        if (_component != null)
        {
            _component.dropdown.interactable = state;
        }
    }

    public override void Dispose()
    {
        if (_component != null)
        {
            _component.dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
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
            if (value is not IEnumerable<string> options) return;
            // Convert to list to avoid multiple enumeration
            var optionsList = options as IList<string> ?? new List<string>(options);
            _options = optionsList;
            _control.SetOptions(optionsList);
        }

        public event Action<object?>? ValueChanged;
    }
}
