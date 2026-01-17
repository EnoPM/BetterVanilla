using System;

namespace BetterVanilla.Ui.Binding.Converters;

/// <summary>
/// Formats a value using string.Format.
/// The format string is passed as the converter parameter.
/// </summary>
public sealed class StringFormatConverter : ValueConverterBase<object, string>
{
    public static StringFormatConverter Instance { get; } = new();

    public override string? Convert(object? value, object? parameter)
    {
        if (value == null)
            return null;

        var format = parameter as string ?? "{0}";
        return string.Format(format, value);
    }
}

/// <summary>
/// Converts an integer to a formatted string.
/// </summary>
public sealed class IntToStringConverter : ValueConverterBase<int, string>
{
    public static IntToStringConverter Instance { get; } = new();

    public override string Convert(int value, object? parameter)
    {
        var format = parameter as string;
        return string.IsNullOrEmpty(format) ? value.ToString() : value.ToString(format);
    }

    public override int ConvertBack(string? value, object? parameter)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}

/// <summary>
/// Converts a float to a formatted string.
/// </summary>
public sealed class FloatToStringConverter : ValueConverterBase<float, string>
{
    public static FloatToStringConverter Instance { get; } = new();

    public override string Convert(float value, object? parameter)
    {
        var format = parameter as string;
        return string.IsNullOrEmpty(format) ? value.ToString() : value.ToString(format);
    }

    public override float ConvertBack(string? value, object? parameter)
    {
        return float.TryParse(value, out var result) ? result : 0f;
    }
}

/// <summary>
/// Passes through the value unchanged. Useful for debugging.
/// </summary>
public sealed class PassThroughConverter : IValueConverter
{
    public static PassThroughConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter) => value;
    public object? ConvertBack(object? value, Type targetType, object? parameter) => value;
}
