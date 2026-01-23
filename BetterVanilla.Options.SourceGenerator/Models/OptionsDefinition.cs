using System.Collections.Generic;

namespace BetterVanilla.Options.SourceGenerator.Models;

public sealed class OptionsDefinition
{
    public string SourceFile { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Namespace { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public List<OptionEntry> Options { get; } = new();
}
