using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BetterVanilla.Options.Core.OptionTypes;

/// <summary>
/// Represents a single choice in an EnumOption.
/// </summary>
public sealed class EnumChoice
{
    public string Value { get; }
    public Func<string> LabelProvider { get; }

    public EnumChoice(string value, Func<string> labelProvider)
    {
        Value = value;
        LabelProvider = labelProvider;
    }

    public string Label => LabelProvider();
}

/// <summary>
/// An option that allows selecting from a list of predefined choices.
/// </summary>
public sealed class EnumOption : OptionBase
{
    private readonly List<EnumChoice> _choices;
    private readonly int _defaultIndex;
    private int _selectedIndex;

    public EnumOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, IEnumerable<EnumChoice> choices, string defaultValue)
        : base(key, labelProvider, descriptionProvider)
    {
        _choices = choices.ToList();
        _defaultIndex = _choices.FindIndex(c => c.Value == defaultValue);
        if (_defaultIndex < 0) _defaultIndex = 0;
        _selectedIndex = _defaultIndex;
    }

    /// <summary>
    /// The list of available choices.
    /// </summary>
    public IReadOnlyList<EnumChoice> Choices => _choices;

    /// <summary>
    /// The currently selected index.
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            var clamped = Math.Clamp(value, 0, _choices.Count - 1);
            if (_selectedIndex == clamped)
            {
                System.Console.WriteLine($"Selected index is clamped ({clamped})");
                return;
            }
            _selectedIndex = clamped;
            OnValueChanged();
        }
    }

    /// <summary>
    /// The currently selected value (string identifier).
    /// </summary>
    public string Value
    {
        get => _choices[_selectedIndex].Value;
        set
        {
            var index = _choices.FindIndex(c => c.Value == value);
            if (index < 0) return;
            SelectedIndex = index;
        }
    }

    /// <summary>
    /// The label of the currently selected choice.
    /// </summary>
    public string SelectedLabel => _choices[_selectedIndex].Label;

    public override void Write(BinaryWriter writer)
    {
        writer.Write(_selectedIndex);
    }

    public override void Read(BinaryReader reader)
    {
        var index = reader.ReadInt32();
        if (index >= 0 && index < _choices.Count)
        {
            _selectedIndex = index;
        }
        else
        {
            _selectedIndex = _defaultIndex;
        }
    }

    public override void Reset()
    {
        SelectedIndex = _defaultIndex;
    }

    public static implicit operator string(EnumOption option) => option.Value;
    public static implicit operator int(EnumOption option) => option.SelectedIndex;
}
