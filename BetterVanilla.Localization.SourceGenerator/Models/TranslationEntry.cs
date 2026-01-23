using System.Collections.Generic;

namespace BetterVanilla.Localization.SourceGenerator.Models;

public sealed class TranslationEntry
{
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, string> Translations { get; } = new();
}
