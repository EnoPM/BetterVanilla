using System;

namespace BetterVanilla.Ui.Binding.Converters;

/// <summary>
/// Converts an enum value to its string representation.
/// </summary>
public sealed class EnumToStringConverter : IValueConverter
{
    public static EnumToStringConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter)
    {
        if (value == null)
            return null;

        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter)
    {
        if (value == null || !targetType.IsEnum)
            return null;

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return null;

        return Enum.Parse(targetType, stringValue, ignoreCase: true);
    }
}

/// <summary>
/// Converts an enum value to its underlying integer value.
/// </summary>
public sealed class EnumToIntConverter : IValueConverter
{
    public static EnumToIntConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter)
    {
        if (value == null)
            return 0;

        return System.Convert.ToInt32(value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter)
    {
        if (value == null || !targetType.IsEnum)
            return null;

        var intValue = System.Convert.ToInt32(value);
        return Enum.ToObject(targetType, intValue);
    }
}

/// <summary>
/// Compares an enum value to a specific value (passed as parameter).
/// Returns true if they match.
/// </summary>
public sealed class EnumToBoolConverter : IValueConverter
{
    public static EnumToBoolConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter)
    {
        if (value == null || parameter == null)
            return false;

        // Parameter can be enum value or string
        if (parameter is string stringParam)
        {
            return string.Equals(value.ToString(), stringParam, StringComparison.OrdinalIgnoreCase);
        }

        return value.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter)
    {
        if (value is not true || parameter == null)
            return null;

        // If checked, return the parameter as the enum value
        if (parameter is string stringParam && targetType.IsEnum)
        {
            return Enum.Parse(targetType, stringParam, ignoreCase: true);
        }

        return parameter;
    }
}
