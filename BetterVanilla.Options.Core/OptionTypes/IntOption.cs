using System;
using System.IO;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class IntOption : OptionBase
{
    private readonly int _defaultValue;
    private readonly int? _min;
    private readonly int? _max;
    private int _value;

    public IntOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, int defaultValue, int? min = null, int? max = null)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _min = min;
        _max = max;
        _value = Clamp(defaultValue);
    }

    public int Value
    {
        get => _value;
        set
        {
            var clamped = Clamp(value);
            if (_value == clamped) return;
            _value = clamped;
            OnValueChanged();
        }
    }

    public int? Min => _min;
    public int? Max => _max;

    public int Clamp(int value)
    {
        if (_min.HasValue && value < _min.Value)
            return _min.Value;
        if (_max.HasValue && value > _max.Value)
            return _max.Value;
        return value;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Read(BinaryReader reader)
    {
        Value = Clamp(reader.ReadInt32());
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator int(IntOption option) => option.Value;
}
