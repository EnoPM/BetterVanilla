using System.Reflection;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Extensions;
using BetterVanilla.Ui;
using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;
using UnityEngine;

namespace BetterVanilla;

[ModInfos(ModData.Guid, ModData.Name, ModData.Version), ModProcess("Among Us")]
public sealed class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; } = null!;

    public Plugin()
    {
        Instance = this;
    }

    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<UnityThreadDispatcher>();
        AddComponent<FeatureCodeBehaviour>();
        AddComponent<BetterVanillaManager>();
        AddComponent<ModUpdaterBehaviour>();
        AddComponent<PlayerShieldBehaviour>();
        
        var bundle = Assembly.GetExecutingAssembly()
            .LoadAssetBundle("BetterVanilla.Assets.manager.bundle");

        var parent = new GameObject($"{nameof(BetterVanilla)}_{nameof(UiManager)}")
        {
            hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset
        };
        
        var manager = bundle.InstantiatePrefab<UiManager>("Assets/BetterVanilla/UiManager.prefab", parent.transform);

        bundle.Unload(false);
        
        Ls.LogMessage($"UiManager loaded state: {manager != null}");
    }
}