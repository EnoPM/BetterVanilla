using System.Collections.Generic;

namespace BetterVanilla.Ui.SourceGenerator.Models;

/// <summary>
/// Represents the parsed view definition.
/// </summary>
public sealed class ViewDefinition
{
    public string ClassName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public ViewElement? RootElement { get; set; }
    public List<ViewElement> NamedElements { get; set; } = new();
    public string SourceFile { get; set; } = string.Empty;

    /// <summary>
    /// The localization source class name (e.g., "MenuLocalization").
    /// Set via LocalizationSource attribute on the View element.
    /// </summary>
    public string? LocalizationSource { get; set; }

    /// <summary>
    /// Whether this view uses localization.
    /// </summary>
    public bool HasLocalization => !string.IsNullOrEmpty(LocalizationSource);
}

/// <summary>
/// Represents a parsed view element from the UI XML.
/// </summary>
public sealed class ViewElement
{
    public string TagName { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Alias { get; set; }
    public List<ViewElement> Children { get; set; } = new();
    public Dictionary<string, string> LiteralProperties { get; set; } = new();
    public Dictionary<string, BindingInfo> Bindings { get; set; } = new();
    public Dictionary<string, string> EventHandlers { get; set; } = new();
    public LayoutInfo Layout { get; set; } = new();
    public TextStyleInfo? TextStyle { get; set; }
    public TextStyleInfo? PlaceholderStyle { get; set; }
    public LayoutGroupInfo? LayoutGroup { get; set; }
    public ImageInfo? Image { get; set; }
    public SourceInfo? Source { get; set; }

    /// <summary>
    /// Localization bindings. Key is the property name (e.g., "Text"), value is the localization key.
    /// Parsed from {Loc Key} syntax.
    /// </summary>
    public Dictionary<string, string> LocalizationBindings { get; set; } = new();

    /// <summary>
    /// Whether this element has any localization bindings.
    /// </summary>
    public bool HasLocalizationBindings => LocalizationBindings.Count > 0;
}

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

public sealed class LayoutInfo
{
    public string? Width { get; set; }
    public string? Height { get; set; }
    public string? MinWidth { get; set; }
    public string? MinHeight { get; set; }
    public string? MaxWidth { get; set; }
    public string? MaxHeight { get; set; }
    public float? FlexibleWidth { get; set; }
    public float? FlexibleHeight { get; set; }
    public string? Margin { get; set; }
    public string? HorizontalAlignment { get; set; }
    public string? VerticalAlignment { get; set; }

    public bool HasAnyValue =>
        Width != null || Height != null ||
        MinWidth != null || MinHeight != null ||
        MaxWidth != null || MaxHeight != null ||
        FlexibleWidth.HasValue || FlexibleHeight.HasValue ||
        Margin != null ||
        HorizontalAlignment != null || VerticalAlignment != null;

    public static (float value, bool isPercentage) ParseDimension(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (0, false);

        var trimmed = value!.Trim();
        if (trimmed.EndsWith("%"))
        {
            if (float.TryParse(trimmed.Substring(0, trimmed.Length - 1), out var percent))
                return (percent, true);
        }
        else if (float.TryParse(trimmed, out var pixels))
        {
            return (pixels, false);
        }

        return (0, false);
    }
}

public sealed class LayoutGroupInfo
{
    public string? Orientation { get; set; }
    public float? Spacing { get; set; }
    public string? Padding { get; set; }
    public string? ChildAlignment { get; set; }
    public bool? ChildControlWidth { get; set; }
    public bool? ChildControlHeight { get; set; }
    public bool? ChildForceExpandWidth { get; set; }
    public bool? ChildForceExpandHeight { get; set; }
    public bool? ReverseArrangement { get; set; }

    public bool HasAnyValue =>
        Orientation != null || Spacing.HasValue || Padding != null || ChildAlignment != null ||
        ChildControlWidth.HasValue || ChildControlHeight.HasValue ||
        ChildForceExpandWidth.HasValue || ChildForceExpandHeight.HasValue ||
        ReverseArrangement.HasValue;
}

public sealed class ImageInfo
{
    public string? Color { get; set; }
    public bool? PreserveAspect { get; set; }
    public string? ImageType { get; set; }
    public SourceInfo? Source { get; set; }

    public bool HasAnyValue =>
        Color != null || PreserveAspect.HasValue || ImageType != null || Source?.HasAnyValue == true;
}

public sealed class SourceInfo
{
    public string? EmbeddedResource { get; set; }
    public string? Url { get; set; }
    public string? FilePath { get; set; }
    public float? PixelsPerUnit { get; set; }
    public string? Pivot { get; set; }
    public string? WrapMode { get; set; }
    public string? FilterMode { get; set; }
    public bool? GenerateMipmaps { get; set; }

    public bool HasAnyValue =>
        EmbeddedResource != null || Url != null || FilePath != null ||
        PixelsPerUnit.HasValue || Pivot != null ||
        WrapMode != null || FilterMode != null || GenerateMipmaps.HasValue;
}

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
