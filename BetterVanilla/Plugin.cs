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
    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<UnityThreadDispatcher>();
        AddComponent<GameEventManager>();
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
        
        bundle.InstantiatePrefab<UiManager>("Assets/BetterVanilla/UiManager.prefab", parent.transform);
        bundle.Unload(false);
    }
}