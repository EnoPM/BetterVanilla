using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using BetterVanilla.BetterModMenu;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Extensions;
using EnoUnityLoader.Il2Cpp.Utils;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterVanillaManager : MonoBehaviour
{
    public static BetterVanillaManager Instance { get; private set; } = null!;

    private readonly Harmony _harmony = new(ModData.Guid);
    public readonly List<BetterPlayerControl> AllPlayers = [];
    public readonly Dictionary<int, TeamPreferences> AllTeamPreferences = [];
    public readonly Dictionary<int, TeamPreferences> AllForcedTeamAssignments = [];
    public ChatCommandsManager ChatCommands { get; private set; } = null!;
    public CheatersManager Cheaters { get; private set; } = null!;
    public ModMenu Menu { get; private set; } = null!;
    public ZoomBehaviourManager? ZoomBehaviour { get; internal set; } = null!;
    public Sprite VentSprite { get; private set; } = null!;
    public BetterPlayerTexts PlayerTextsPrefab { get; private set; } = null!;
    public BetterPlayerTexts BetterVoteAreaTextsPrefab { get; private set; } = null!;

    private void Awake()
    {
        if (Instance) throw new Exception($"{nameof(BetterVanillaManager)} must be a singleton");
        Instance = this;

        ChatCommands = new ChatCommandsManager();
        Cheaters = new CheatersManager();
        Menu = new ModMenu();

        var betterGame = Assembly.GetExecutingAssembly()
            .LoadAssetBundle("BetterVanilla.Assets.better.game");

        VentSprite = betterGame.LoadAsset<Sprite>("Assets/Ui/Icons/Vent.png");
        VentSprite.hideFlags = HideFlags.HideAndDontSave;

        PlayerTextsPrefab = betterGame.InstantiatePrefab<BetterPlayerTexts>("Assets/Ui/BetterPlayerTexts.prefab", transform);
        PlayerTextsPrefab.gameObject.SetActive(false);

        BetterVoteAreaTextsPrefab = betterGame.InstantiatePrefab<BetterPlayerTexts>("Assets/Ui/BetterVoteAreaTexts.prefab", transform);
        BetterVoteAreaTextsPrefab.gameObject.SetActive(false);

        betterGame.Unload(false);

        GameEventManager.Instance.PlayerJoined += OnPlayerJoined;

        Ls.LogInfo($"Plugin {ModData.Name} v{ModData.Version} is loaded!");
    }

    private void Start()
    {
        try
        {
            _harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Ls.LogWarning($"Unable to apply patches: {ex.Message}");
            throw;
        }

        this.StartCoroutine(CoStart());
        this.StartCoroutine(CoLoadFeaturesConfig());
    }

    private IEnumerator CoLoadFeaturesConfig()
    {
        yield return FeaturesManager.CoLoad();
        Ls.LogMessage($"[JSON] {JsonSerializer.Serialize(FeaturesManager.Instance)}");
    }

    private IEnumerator CoStart()
    {
        yield return new WaitForSeconds(5f);
        while (!ModManager.InstanceExists)
        {
            yield return new WaitForSeconds(1f);
        }

        ModManager.Instance.ShowModStamp();

        yield return CoLoadCosmetics();

        Ls.LogInfo($"Plugin {ModData.Name} v{ModData.Version} was successfully started!");
    }

    private IEnumerator CoLoadCosmetics()
    {
        Ls.LogInfo("Loading cosmetics bundle...");
        var loader = gameObject.AddComponent<CosmeticsLoader>();
        yield return loader.CoLoadCosmetics();
        Destroy(loader);
    }

    public BetterPlayerControl? GetPlayerById(byte playerId)
    {
        return AllPlayers.Find(x => x.Player.PlayerId == playerId);
    }

    public BetterPlayerControl? GetPlayerByOwnerId(int ownerId)
    {
        return AllPlayers.Find(x => x.Player.OwnerId == ownerId);
    }

    public BetterPlayerControl? GetPlayerByFriendCode(string friendCode)
    {
        return AllPlayers.Find(x => x.FriendCode != null && x.FriendCode == friendCode);
    }

    private static void OnPlayerJoined(PlayerControl player)
    {
        player.gameObject.AddComponent<BetterPlayerControl>();
    }
}