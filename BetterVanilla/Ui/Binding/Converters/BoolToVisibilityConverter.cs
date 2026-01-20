namespace BetterVanilla.Ui.Binding.Converters;

/// <summary>
/// Converts a boolean value to a visibility state.
/// The converter parameter can invert the logic ("Invert" or "true").
/// </summary>
public sealed class BoolToVisibilityConverter : ValueConverterBase<bool, bool>
{
    public static BoolToVisibilityConverter Instance { get; } = new();

    public override bool Convert(bool value, object? parameter)
    {
        var invert = ShouldInvert(parameter);
        return invert ? !value : value;
    }

    public override bool ConvertBack(bool value, object? parameter)
    {
        var invert = ShouldInvert(parameter);
        return invert ? !value : value;
    }

    private static bool ShouldInvert(object? parameter)
    {
        if (parameter == null)
            return false;

        var paramStr = parameter.ToString()?.ToLowerInvariant();
        return paramStr is "invert" or "true" or "1";
    }
}

/// <summary>
/// Converts a boolean to an alpha value (0 or 1).
/// </summary>
public sealed class BoolToAlphaConverter : ValueConverterBase<bool, float>
{
    public static BoolToAlphaConverter Instance { get; } = new();

    public float TrueAlpha { get; set; } = 1f;
    public float FalseAlpha { get; set; } = 0f;

    public override float Convert(bool value, object? parameter)
    {
        return value ? TrueAlpha : FalseAlpha;
    }

    public override bool ConvertBack(float value, object? parameter)
    {
        return value > 0.5f;
    }
}
