using System.Collections.Generic;

namespace BetterVanilla.Options.SourceGenerator.Models;

public sealed class OptionEntry
{
    public string Name { get; set; } = string.Empty;
    public OptionEntryType Type { get; set; }
    public string? Default { get; set; }
    public string? Min { get; set; }
    public string? Max { get; set; }
    public int? MaxLength { get; set; }
    public string? EnumType { get; set; }

    /// <summary>
    /// The localization key for the option label (legacy attribute mode).
    /// If not specified and no LabelTranslations, defaults to {Name}Label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Inline translations for the option label.
    /// Key: language code (e.g., "En", "Fr"), Value: translated text.
    /// </summary>
    public Dictionary<string, string> LabelTranslations { get; } = new();

    /// <summary>
    /// Inline translations for the option description.
    /// Key: language code (e.g., "En", "Fr"), Value: translated text.
    /// </summary>
    public Dictionary<string, string> DescriptionTranslations { get; } = new();

    /// <summary>
    /// Whether this option has inline label translations.
    /// </summary>
    public bool HasInlineLabel => LabelTranslations.Count > 0;

    /// <summary>
    /// Whether this option has inline description translations.
    /// </summary>
    public bool HasInlineDescription => DescriptionTranslations.Count > 0;
}

public enum OptionEntryType
{
    Bool,
    Int,
    Float,
    String,
    Enum,
    Color,
    Vector2
}
