using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data;

// ReSharper disable once ClassNeverInstantiated.Global
public class GithubRelease
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("draft")]
    public bool Draft { get; set; }
    
    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }
    
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = null!;
    
    [JsonPropertyName("published_at")]
    public string PublishedAt { get; set; } = null!;
    
    [JsonPropertyName("body")]
    public string Description { get; set; } = null!;
    
    [JsonPropertyName("assets")]
    public List<GithubAsset> Assets { get; set; } = null!;

    internal Version Version => Version.Parse(Tag.Replace("v", string.Empty));

    public bool IsNewer(Version version)
    {
        return Version > version;
    }

    public bool IsOlder(Version version)
    {
        return Version < version;
    }
}