using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BetterVanilla.Core;

public sealed class FeaturesManager
{
    public static FeaturesManager? Instance { get; private set; }

    public static IEnumerator CoLoad()
    {
        var readFileTask = File.ReadAllTextAsync(@"C:\projects\BetterVanilla2\features.yml");
        while (!readFileTask.IsCompleted)
        {
            yield return null;
        }

        var fileContent = readFileTask.Result;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        Instance = deserializer.Deserialize<FeaturesManager>(fileContent);
    }

    public SponsorsConfig Sponsors { get; set; } = new();
    public Dictionary<string, List<string>> Features { get; set; } = new();
    
    [YamlMember(Alias = "private_cosmetics")]
    public Dictionary<string, List<string>> PrivateCosmetics { get; set; } = new();

    [YamlMember(Alias = "cosmetics_bundles")]
    public List<CosmeticBundle> CosmeticBundles { get; set; } = [];

    public sealed class SponsorsConfig
    {
        public List<string> Players { get; set; } = [];
        public List<string> Cosmetics { get; set; } = [];
    }

    public sealed class CosmeticBundle
    {
        public string Hash { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}