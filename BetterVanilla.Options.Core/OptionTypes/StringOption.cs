using System;
using System.IO;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class StringOption : OptionBase
{
    private readonly string _defaultValue;
    private readonly int? _maxLength;
    private string _value;

    public StringOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, string defaultValue, int? maxLength = null)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _maxLength = maxLength;
        _value = Truncate(defaultValue);
    }

    public string Value
    {
        get => _value;
        set
        {
            var truncated = Truncate(value ?? string.Empty);
            if (_value == truncated) return;
            _value = truncated;
            OnValueChanged();
        }
    }

    public int? MaxLength => _maxLength;

    private string Truncate(string value)
    {
        if (_maxLength.HasValue && value.Length > _maxLength.Value)
            return value.Substring(0, _maxLength.Value);
        return value;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(_value);
    }

    public override void Read(BinaryReader reader)
    {
        _value = Truncate(reader.ReadString());
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator string(StringOption option) => option.Value;
}
