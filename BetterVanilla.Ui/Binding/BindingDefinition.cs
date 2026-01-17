using System;

namespace BetterVanilla.Ui.Binding;

/// <summary>
/// Defines the direction of data flow in a binding.
/// </summary>
public enum BindingMode
{
    /// <summary>
    /// Data flows from source to target only (ViewModel to UI).
    /// </summary>
    OneWay,

    /// <summary>
    /// Data flows in both directions (ViewModel <-> UI).
    /// </summary>
    TwoWay,

    /// <summary>
    /// Data flows from source to target once at initialization.
    /// </summary>
    OneTime,

    /// <summary>
    /// Data flows from target to source only (UI to ViewModel).
    /// </summary>
    OneWayToSource
}

/// <summary>
/// Defines when the binding updates the source value.
/// </summary>
public enum UpdateSourceTrigger
{
    /// <summary>
    /// Update source when the target property changes.
    /// </summary>
    PropertyChanged,

    /// <summary>
    /// Update source when the target loses focus.
    /// </summary>
    LostFocus,

    /// <summary>
    /// Update source only when explicitly requested.
    /// </summary>
    Explicit
}

/// <summary>
/// Defines a binding between a source property and a target property.
/// </summary>
public sealed class BindingDefinition
{
    /// <summary>
    /// The path to the source property (e.g., "User.Name" or "IsEnabled").
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// The name of the target property on the control.
    /// </summary>
    public string TargetProperty { get; }

    /// <summary>
    /// The binding mode.
    /// </summary>
    public BindingMode Mode { get; set; } = BindingMode.OneWay;

    /// <summary>
    /// When to update the source for TwoWay bindings.
    /// </summary>
    public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.PropertyChanged;

    /// <summary>
    /// Optional converter to transform values between source and target.
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Optional parameter passed to the converter.
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Fallback value when binding fails or source value is null.
    /// </summary>
    public object? FallbackValue { get; set; }

    /// <summary>
    /// String format to apply to the value (e.g., "{0:N2}").
    /// </summary>
    public string? StringFormat { get; set; }

    public BindingDefinition(string sourcePath, string targetProperty)
    {
        SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
        TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
    }

    /// <summary>
    /// Creates a OneWay binding.
    /// </summary>
    public static BindingDefinition OneWay(string sourcePath, string targetProperty)
    {
        return new BindingDefinition(sourcePath, targetProperty) { Mode = BindingMode.OneWay };
    }

    /// <summary>
    /// Creates a TwoWay binding.
    /// </summary>
    public static BindingDefinition TwoWay(string sourcePath, string targetProperty)
    {
        return new BindingDefinition(sourcePath, targetProperty) { Mode = BindingMode.TwoWay };
    }

    /// <summary>
    /// Creates a OneTime binding.
    /// </summary>
    public static BindingDefinition OneTime(string sourcePath, string targetProperty)
    {
        return new BindingDefinition(sourcePath, targetProperty) { Mode = BindingMode.OneTime };
    }

    public override string ToString()
    {
        return $"Binding: {SourcePath} -> {TargetProperty} ({Mode})";
    }
}

/// <summary>
/// Represents a parsed binding expression from XML (e.g., "{Binding IsEnabled, Mode=TwoWay}").
/// </summary>
public sealed class BindingExpression
{
    public string Path { get; set; } = string.Empty;
    public BindingMode Mode { get; set; } = BindingMode.OneWay;
    public string? ConverterName { get; set; }
    public string? ConverterParameter { get; set; }
    public string? StringFormat { get; set; }
    public string? FallbackValue { get; set; }

    /// <summary>
    /// Parses a binding expression string.
    /// </summary>
    public static BindingExpression? Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return null;

        var trimmed = expression.Trim();

        // Check if it's a binding expression
        if (!trimmed.StartsWith("{Binding", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!trimmed.EndsWith("}"))
            return null;

        // Remove {Binding and }
        var content = trimmed.Substring(8, trimmed.Length - 9).Trim();

        var result = new BindingExpression();

        // Parse the content
        var parts = SplitBindingContent(content);
        var isFirstPart = true;

        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();
            if (string.IsNullOrEmpty(trimmedPart))
                continue;

            // First part without '=' is the path
            if (isFirstPart && !trimmedPart.Contains('='))
            {
                result.Path = trimmedPart;
                isFirstPart = false;
                continue;
            }

            isFirstPart = false;

            // Parse key=value pairs
            var eqIndex = trimmedPart.IndexOf('=');
            if (eqIndex <= 0)
                continue;

            var key = trimmedPart.Substring(0, eqIndex).Trim();
            var value = trimmedPart.Substring(eqIndex + 1).Trim();

            switch (key.ToLowerInvariant())
            {
                case "path":
                    result.Path = value;
                    break;
                case "mode":
                    if (Enum.TryParse<BindingMode>(value, true, out var mode))
                        result.Mode = mode;
                    break;
                case "converter":
                    result.ConverterName = value;
                    break;
                case "converterparameter":
                    result.ConverterParameter = value;
                    break;
                case "stringformat":
                    result.StringFormat = value;
                    break;
                case "fallbackvalue":
                    result.FallbackValue = value;
                    break;
            }
        }

        return string.IsNullOrEmpty(result.Path) ? null : result;
    }

    private static string[] SplitBindingContent(string content)
    {
        var parts = new System.Collections.Generic.List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var c in content)
        {
            if (c == '\'' || c == '"')
            {
                inQuotes = !inQuotes;
                current.Append(c);
            }
            else if (c == ',' && !inQuotes)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            parts.Add(current.ToString());

        return parts.ToArray();
    }

    /// <summary>
    /// Checks if a string is a binding expression.
    /// </summary>
    public static bool IsBindingExpression(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();
        return trimmed.StartsWith("{Binding", StringComparison.OrdinalIgnoreCase) && trimmed.EndsWith("}");
    }
}
