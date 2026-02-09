using System.Collections;
using System.Collections.Generic;
using System.IO;
using BetterVanilla.Components;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BetterVanilla.Core;

public sealed class FeaturesManager
{
    public static FeaturesManager? Instance { get; private set; }

    public static IEnumerator CoLoad()
    {
        yield return CoLoadFromFile(@"C:\projects\BetterVanilla2\features.yml");
    }

    private static IEnumerator CoLoadFromFile(string filePath)
    {
        var readFileTask = File.ReadAllTextAsync(filePath);
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

    [YamlMember(Alias = "cosmetic_bundles")]
    public List<CosmeticBundle> CosmeticBundles { get; set; } = [];

    public bool IsCosmeticUnlocked(string productId)
    {
        var friendCode = EOSManager.InstanceExists && !string.IsNullOrEmpty(EOSManager.Instance.FriendCode) ? EOSManager.Instance.FriendCode : null;
        if (Sponsors.Cosmetics.Contains(productId))
        {
            return friendCode != null && Sponsors.Players.Contains(friendCode);
        }
        if (PrivateCosmetics.TryGetValue(productId, out var allowedPlayers))
        {
            return friendCode != null && allowedPlayers.Contains(friendCode);
        }
        return true;
    }

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