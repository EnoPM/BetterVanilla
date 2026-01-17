namespace BetterVanilla.Ui.XamlGenerator.Models;

/// <summary>
/// Represents a parsed view element from the BVUI XML.
/// </summary>
public sealed class ViewElement
{
    /// <summary>
    /// The tag name (e.g., "Button", "Panel").
    /// </summary>
    public string TagName { get; set; } = string.Empty;

    /// <summary>
    /// The x:Name attribute value.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The Alias attribute value (e.g., "Controls/Button").
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// Child elements.
    /// </summary>
    public List<ViewElement> Children { get; set; } = [];

    /// <summary>
    /// Property values that are literal values.
    /// </summary>
    public Dictionary<string, string> LiteralProperties { get; set; } = new();

    /// <summary>
    /// Property values that are binding expressions.
    /// </summary>
    public Dictionary<string, BindingInfo> Bindings { get; set; } = new();

    /// <summary>
    /// Event handlers (e.g., Click="OnButtonClicked").
    /// </summary>
    public Dictionary<string, string> EventHandlers { get; set; } = new();
}

/// <summary>
/// Represents binding information parsed from a binding expression.
/// </summary>
public sealed class BindingInfo
{
    public string Path { get; set; } = string.Empty;
    public string Mode { get; set; } = "OneWay";
    public string? Converter { get; set; }
    public string? ConverterParameter { get; set; }
    public string? StringFormat { get; set; }
    public string? FallbackValue { get; set; }

    public string TargetProperty { get; set; } = string.Empty;
}

/// <summary>
/// Represents the parsed view definition.
/// </summary>
public sealed class ViewDefinition
{
    /// <summary>
    /// The x:Class attribute (full type name including namespace).
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// The namespace extracted from x:Class.
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// The class name without namespace.
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// The root element of the view.
    /// </summary>
    public ViewElement? RootElement { get; set; }

    /// <summary>
    /// All named elements in the view.
    /// </summary>
    public List<ViewElement> NamedElements { get; set; } = [];

    /// <summary>
    /// Source file path.
    /// </summary>
    public string SourceFile { get; set; } = string.Empty;
}
