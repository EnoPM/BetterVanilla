using System;
using System.IO;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class BoolOption : OptionBase
{
    private readonly bool _defaultValue;
    private bool _value;

    public BoolOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, bool defaultValue)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
    }

    public bool Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged();
        }
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        Value = reader.ReadBoolean();
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator bool(BoolOption option) => option.Value;
}
