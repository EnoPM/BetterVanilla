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

    /// <summary>
    /// Layout properties for this element.
    /// </summary>
    public LayoutInfo Layout { get; set; } = new();

    /// <summary>
    /// Text style properties (for controls with text labels like Button, Toggle, Slider, InputField).
    /// </summary>
    public TextStyleInfo? TextStyle { get; set; }

    /// <summary>
    /// Placeholder text style properties (for InputField).
    /// </summary>
    public TextStyleInfo? PlaceholderStyle { get; set; }
}

/// <summary>
/// Represents text style properties for TMP_Text components.
/// </summary>
public sealed class TextStyleInfo
{
    public float? FontSize { get; set; }
    public string? TextColor { get; set; }
    public string? TextAlignment { get; set; }
    public string? FontStyle { get; set; }
    public float? CharacterSpacing { get; set; }
    public float? LineSpacing { get; set; }
    public float? WordSpacing { get; set; }
    public bool? WordWrapping { get; set; }
    public string? TextOverflow { get; set; }
    public bool? RichText { get; set; }
    public bool? AutoSize { get; set; }
    public float? MinFontSize { get; set; }
    public float? MaxFontSize { get; set; }

    public string? TextMargin { get; set; }

    public bool HasAnyValue =>
        FontSize.HasValue || TextColor != null || TextAlignment != null ||
        FontStyle != null || CharacterSpacing.HasValue || LineSpacing.HasValue ||
        WordSpacing.HasValue || WordWrapping.HasValue || TextOverflow != null ||
        RichText.HasValue || AutoSize.HasValue || MinFontSize.HasValue || MaxFontSize.HasValue ||
        TextMargin != null;
}

/// <summary>
/// Represents layout properties parsed from the BVUI XML.
/// </summary>
public sealed class LayoutInfo
{
    // Base layout properties
    public float? Width { get; set; }
    public float? Height { get; set; }
    public float? MinWidth { get; set; }
    public float? MinHeight { get; set; }
    public float? MaxWidth { get; set; }
    public float? MaxHeight { get; set; }
    public float? FlexibleWidth { get; set; }
    public float? FlexibleHeight { get; set; }
    public string? Margin { get; set; }
    public string? Padding { get; set; }
    public string? HorizontalAlignment { get; set; }
    public string? VerticalAlignment { get; set; }

    // LayoutGroup properties (for Panel)
    public string? Background { get; set; }
    public string? Orientation { get; set; }
    public float? Spacing { get; set; }
    public string? ChildAlignment { get; set; }
    public bool? ChildControlWidth { get; set; }
    public bool? ChildControlHeight { get; set; }
    public bool? ChildForceExpandWidth { get; set; }
    public bool? ChildForceExpandHeight { get; set; }
    public bool? ReverseArrangement { get; set; }

    public bool HasAnyValue =>
        Width.HasValue || Height.HasValue ||
        MinWidth.HasValue || MinHeight.HasValue ||
        MaxWidth.HasValue || MaxHeight.HasValue ||
        FlexibleWidth.HasValue || FlexibleHeight.HasValue ||
        Margin != null || Padding != null ||
        HorizontalAlignment != null || VerticalAlignment != null ||
        Background != null || Orientation != null || Spacing.HasValue || ChildAlignment != null ||
        ChildControlWidth.HasValue || ChildControlHeight.HasValue ||
        ChildForceExpandWidth.HasValue || ChildForceExpandHeight.HasValue ||
        ReverseArrangement.HasValue;
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
