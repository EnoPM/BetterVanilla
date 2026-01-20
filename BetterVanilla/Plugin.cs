using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Ui;
using BetterVanilla.Ui.Core;
using BetterVanilla.Views;
using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;

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
        UiLogger.SetLogSource(Log);
        AddComponent<UnityThreadDispatcher>();
        AddComponent<FeatureCodeBehaviour>();
        AddComponent<BetterVanillaManager>();
        AddComponent<ModUpdaterBehaviour>();
        AddComponent<PlayerShieldBehaviour>();

        UiManager.Instance.CreateView<MenuButtonOverlay, MenuButtonOverlayViewModel>();
    }
}