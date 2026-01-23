using System.Collections.Generic;

namespace BetterVanilla.Localization.SourceGenerator.Models;

public sealed class LocalizationDefinition
{
    public string SourceFile { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Namespace { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = "En";
    public List<string> Languages { get; } = new();
    public List<TranslationEntry> Entries { get; } = new();
}
