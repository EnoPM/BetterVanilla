using System;
using System.IO;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class FloatOption : OptionBase
{
    private readonly float _defaultValue;
    private readonly float? _min;
    private readonly float? _max;
    private readonly float? _step;
    private readonly string? _prefix;
    private readonly string? _suffix;
    private float _value;

    public FloatOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, float defaultValue, float? min = null, float? max = null, float? step = null, string? prefix = null, string? suffix = null)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _min = min;
        _max = max;
        _step = step;
        _prefix = prefix;
        _suffix = suffix;
        _value = Clamp(defaultValue);
    }

    public float Value
    {
        get => _value;
        set
        {
            var clamped = Clamp(value);
            if (System.Math.Abs(_value - clamped) < float.Epsilon) return;
            _value = clamped;
            OnValueChanged();
        }
    }

    public float? Min => _min;
    public float? Max => _max;
    public float? Step => _step;
    public string? Prefix => _prefix;
    public string? Suffix => _suffix;

    public float Clamp(float value)
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
        Value = Clamp(reader.ReadSingle());
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator float(FloatOption option) => option.Value;
}
