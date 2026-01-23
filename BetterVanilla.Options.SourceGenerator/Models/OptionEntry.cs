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
