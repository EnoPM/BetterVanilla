using System;
using System.IO;
using BetterVanilla.Core.Options;
using BetterVanilla.Localization;
using BetterVanilla.Options;
using BetterVanilla.Options.Core;

namespace BetterVanilla.Core;

public static class ModOptions
{
    public static OptionsManager<UserOptions> User { get; }
    public static OptionsManager<GameLocalOptions> GameLocal { get; }
    public static OptionsManager<VanillaHostOptions> VanillaHost { get; }
    public static OptionsManager<GameHostOptions> GameHost { get; }

    static ModOptions()
    {
        User = new OptionsManager<UserOptions>(Path.Combine(PathsManager.Instance.OptionsDirectory, "user.dat"));
        
        GameLocal = new OptionsManager<GameLocalOptions>(Path.Combine(PathsManager.Instance.OptionsDirectory, "game_local.dat"));
        
        VanillaHost = new VanillaHostOptionsManager(Path.Combine(PathsManager.Instance.OptionsDirectory, "vanilla_host.dat"));
        
        GameHost = new GameHostOptionsManager(Path.Combine(PathsManager.Instance.OptionsDirectory, "game_host.dat"));

        if (Enum.TryParse<Language>(User.Options.Language.Value, out var language))
        {
            LocalizationManager.CurrentLanguage = language;
        }
    }
}