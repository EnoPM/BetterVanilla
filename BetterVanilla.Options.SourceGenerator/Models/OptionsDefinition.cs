using System.Collections.Generic;
using System.Linq;

namespace BetterVanilla.Options.SourceGenerator.Models;

public sealed class OptionsDefinition
{
    public string SourceFile { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Namespace { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public List<OptionEntry> Options { get; } = new();

    /// <summary>
    /// The localization source class to use for option labels (legacy mode).
    /// Example: "BetterVanilla.Localization.MenuLocalization"
    /// </summary>
    public string? LocalizationSource { get; set; }

    /// <summary>
    /// The default language for inline translations (e.g., "En").
    /// </summary>
    public string DefaultLanguage { get; set; } = "En";

    /// <summary>
    /// All languages used across all options' inline translations.
    /// Sorted with default language first.
    /// </summary>
    public List<string> Languages { get; } = new();

    /// <summary>
    /// Whether this options holder has legacy localization support (via LocalizationSource).
    /// </summary>
    public bool HasLocalization => !string.IsNullOrEmpty(LocalizationSource);

    /// <summary>
    /// Whether any option has inline translations (Label or Description).
    /// </summary>
    public bool HasInlineTranslations => Options.Any(o => o.HasInlineLabel || o.HasInlineDescription);
}
