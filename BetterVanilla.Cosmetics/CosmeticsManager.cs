using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using BetterVanilla.Cosmetics.Hats;
using BetterVanilla.Cosmetics.Visors;
using EnoUnityLoader.Logging;
using HarmonyLib;

namespace BetterVanilla.Cosmetics;

public sealed class CosmeticsManager
{
    public static readonly CosmeticsManager Instance = new();
    internal ManualLogSource Log { get; }

    public HatCosmeticManager Hats { get; }
    public VisorCosmeticManager Visors { get; }

    static CosmeticsManager()
    {
        var harmony = new Harmony($"pm.eno.cosmetics.bettervanilla");
        harmony.PatchAll(typeof(CosmeticsManager).Assembly);
    }

    private CosmeticsManager()
    {
        Log = EnoUnityLoader.Logging.Logger.CreateLogSource("BetterVanilla.Cosmetics");
        Hats = new HatCosmeticManager();
        Visors = new VisorCosmeticManager();
    }
    
    public void RegisterBundle(CosmeticBundle bundle)
    {
        var cache = new SpritesheetCache(bundle.AllSpritesheet);
        
        Log.LogInfo($"[Hats] Registering {bundle.Hats.Count} cosmetics");
        foreach (var serialized in bundle.Hats)
        {
            var cosmetic = new HatCosmetic(serialized, cache);
            Hats.AddCosmetic(cosmetic);
        }
        
        Log.LogInfo($"[Visors] Registering {bundle.Visors.Count} cosmetics");
        foreach (var serialized in bundle.Visors)
        {
            var cosmetic = new VisorCosmetic(serialized, cache);
            Visors.AddCosmetic(cosmetic);
        }
    }
    
    #region Helpers

    public void UpdateAnimationFrames()
    {
        Hats.UpdateAnimationFrames();
        Visors.UpdateAnimationFrames();
    }

    public void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        Hats.RefreshAnimationFrames(playerPhysics);
        Visors.RefreshAnimationFrames(playerPhysics);
    }

    public void ProcessUnregisteredCosmetics()
    {
        Hats.RegisterCosmetics();
        Visors.RegisterCosmetics();
    }

    #endregion
}