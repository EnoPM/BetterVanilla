using System.Text.Json.Serialization;

namespace BetterVanilla.Ui.XamlGenerator.Models;

/// <summary>
/// Root configuration loaded from ui-aliases.json.
/// </summary>
public sealed class AliasConfig
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("defaultBundle")]
    public string? DefaultBundle { get; set; }

    [JsonPropertyName("defaultNamespace")]
    public string? DefaultNamespace { get; set; }

    [JsonPropertyName("aliases")]
    public Dictionary<string, AliasDefinition> Aliases { get; set; } = new();
}

/// <summary>
/// Definition of a single alias.
/// </summary>
public sealed class AliasDefinition
{
    [JsonPropertyName("prefab")]
    public string Prefab { get; set; } = string.Empty;

    [JsonPropertyName("component")]
    public string Component { get; set; } = string.Empty;

    [JsonPropertyName("bundle")]
    public string? Bundle { get; set; }

    [JsonPropertyName("defaultProperties")]
    public Dictionary<string, object>? DefaultProperties { get; set; }
}
