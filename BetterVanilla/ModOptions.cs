using System.IO;
using BetterVanilla.Core;
using BetterVanilla.Localization;
using BetterVanilla.Options;
using BetterVanilla.Options.Core;

namespace BetterVanilla;

public static class ModOptions
{
    public static readonly OptionsManager<UserOptions> User;
    public static readonly OptionsManager<GameLocalOptions> GameLocal;
    public static readonly OptionsManager<GameHostOptions> GameHost;

    static ModOptions()
    {
        User = new OptionsManager<UserOptions>(Path.Combine(ModPaths.OptionsDirectory, "user.dat"));
        if (System.Enum.TryParse<Language>(User.Options.Language.Value, out var lang))
        {
            LocalizationManager.CurrentLanguage = lang;
        }

        GameLocal = new OptionsManager<GameLocalOptions>(Path.Combine(ModPaths.OptionsDirectory, "game_local.dat"));
        GameHost = new OptionsManager<GameHostOptions>(Path.Combine(ModPaths.OptionsDirectory, "game_host.dat"));
    }
}