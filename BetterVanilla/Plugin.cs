using System.IO;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Localization;
using BetterVanilla.Options;
using BetterVanilla.Options.Core;
using BetterVanilla.Ui;
using BetterVanilla.Views.MenuButtonOverlay;
using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;

namespace BetterVanilla;

[ModInfos(ModData.Guid, ModData.Name, ModData.Version), ModProcess("Among Us")]
public sealed class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; } = null!;

    public static readonly OptionsManager<UserOptions> User;
    public static readonly OptionsManager<GameLocalOptions> GameLocal;

    static Plugin()
    {
        User = new OptionsManager<UserOptions>(Path.Combine(ModPaths.OptionsDirectory, "user.dat"));
        LocalizationManager.CurrentLanguage = User.Options.Language.Value;
        
        GameLocal = new OptionsManager<GameLocalOptions>(Path.Combine(ModPaths.OptionsDirectory, "game_local.dat"));
    }

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

        UiManager.Instance.CreateView<MenuButtonOverlayUi, MenuButtonOverlayUiViewModel>();
    }
}