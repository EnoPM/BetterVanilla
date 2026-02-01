using System;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class Vector2Option : OptionBase
{
    private readonly Vector2 _defaultValue;
    private Vector2 _value;

    public Vector2Option(string key, Func<string> labelProvider, Func<string> descriptionProvider, Vector2 defaultValue)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
    }

    public Vector2 Value
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
        writer.Write(_value.x);
        writer.Write(_value.y);
    }

    public override void Read(BinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        _value = new Vector2(x, y);
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator Vector2(Vector2Option option) => option.Value;

    public static Vector2 ParseVector2(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Vector2.zero;

        var parts = value.Split(',');
        if (parts.Length != 2)
            return Vector2.zero;

        if (float.TryParse(parts[0].Trim(), out var x) && float.TryParse(parts[1].Trim(), out var y))
            return new Vector2(x, y);

        return Vector2.zero;
    }
}
