using System;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Options.Core.OptionTypes;

public sealed class ColorOption : OptionBase
{
    private readonly Color _defaultValue;
    private Color _value;

    public ColorOption(string key, Func<string> labelProvider, Func<string> descriptionProvider, Color defaultValue)
        : base(key, labelProvider, descriptionProvider)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
    }

    public Color Value
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
        writer.Write(Value.r);
        writer.Write(Value.g);
        writer.Write(Value.b);
        writer.Write(Value.a);
    }

    public override void Read(BinaryReader reader)
    {
        var r = reader.ReadSingle();
        var g = reader.ReadSingle();
        var b = reader.ReadSingle();
        var a = reader.ReadSingle();
        Value = new Color(r, g, b, a);
    }

    public override void Reset()
    {
        Value = _defaultValue;
    }

    public static implicit operator Color(ColorOption option) => option.Value;

    public static Color ParseColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.white;

        hex = hex.TrimStart('#');

        float r, g, b, a = 1f;

        switch (hex.Length)
        {
            case 3: // RGB
                r = int.Parse(hex[..1], System.Globalization.NumberStyles.HexNumber) / 15f;
                g = int.Parse(hex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber) / 15f;
                b = int.Parse(hex.Substring(2, 1), System.Globalization.NumberStyles.HexNumber) / 15f;
                break;
            case 6: // RRGGBB
                r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber) / 255f;
                g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                break;
            case 8: // RRGGBBAA
                r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber) / 255f;
                g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                a = int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                break;
            default:
                return Color.white;
        }

        return new Color(r, g, b, a);
    }
}
