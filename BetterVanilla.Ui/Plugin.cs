using BetterVanilla.Ui.Helpers;
using BetterVanilla.Ui.Views;
using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;
using EnoUnityLoader.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

[ModInfos(ModData.Guid, ModData.Name, ModData.Version), ModProcess("Among Us")]
public sealed class Plugin : BasePlugin
{
    internal static Plugin Instance { get; private set; } = null!;

    public Plugin()
    {
        Instance = this;
    }

    public override void Load()
    {
        // Initialize the AssetBundleManager with the UI bundle
        // This would typically load the bundle containing UI prefabs
        // AssetBundleManager.Instance.LoadFromEmbeddedResource("BetterVanilla.Ui.Assets.ui.bundle");

        // Example of creating a view with ViewModel:
        // var optionsView = new GameObject("OptionsView").AddComponent<Views.OptionsView>();
        // optionsView.DataContext = new Views.OptionsViewModel
        // {
        //     IsFeatureEnabled = true,
        //     Volume = 0.75f,
        //     Username = "Player"
        // };

        // 1. Charger le bundle depuis les embedded resources                                                                                                                                                                                                                                                    
        /*
        AssetBundleManager.Instance.LoadFromEmbeddedResource("BetterVanilla.Ui.Assets.ui.bundle");

        UiManager.Instance.CreateView<OptionsView, OptionsViewModel>(new OptionsViewModel
        {
            IsFeatureEnabled = true,
            Volume = 0.75f,
            Username = "Player"
        });
        */
        AddComponent<ComponentDebuggerBehaviour>();
    }
}