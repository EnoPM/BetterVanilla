using System;
using System.IO;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class EnumOption<TEnum> : OptionBase where TEnum : struct, Enum
{
    private readonly TEnum _defaultValue;
    private TEnum _value;

    public EnumOption(string key, TEnum defaultValue) : base(key)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
    }

    public TEnum Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;
            _value = value;
            OnValueChanged();
        }
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(Convert.ToInt32(_value));
    }

    public override void Read(BinaryReader reader)
    {
        var intValue = reader.ReadInt32();
        if (Enum.IsDefined(typeof(TEnum), intValue))
        {
            _value = (TEnum)Enum.ToObject(typeof(TEnum), intValue);
        }
        else
        {
            _value = _defaultValue;
        }
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator TEnum(EnumOption<TEnum> option) => option.Value;
}
