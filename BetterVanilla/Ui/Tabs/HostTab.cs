using BetterVanilla.Core.Extensions;
using BetterVanilla.Ui.Base;

namespace BetterVanilla.Ui.Tabs;

public sealed class HostTab : TabBase
{

    private void Start()
    {
        this.AddOption(ModOptions.GameHost.Options.AllowDeadVoteDisplay);
        this.AddOption(ModOptions.GameHost.Options.AllowTeamPreference);
        this.AddOption(ModOptions.GameHost.Options.HideDeadPlayerPets);
        this.AddOption(ModOptions.GameHost.Options.PolusReactorCountdown);
        this.AddOption(ModOptions.GameHost.Options.ProtectFirstKilledPlayer);
        this.AddOption(ModOptions.GameHost.Options.ProtectionDuration);
        this.AddOption(ModOptions.GameHost.Options.DefineCommonTasksAsNonCommon);
        this.AddOption(ModOptions.GameHost.Options.BetterPolus);
        this.AddOption(ModOptions.GameHost.Options.RandomizeFixWiringTaskOrder);
        this.AddOption(ModOptions.GameHost.Options.RandomizeUploadTaskLocation);
        this.AddOption(ModOptions.GameHost.Options.RandomizePlayerOrderInMeetings);
        this.AddOption(ModOptions.GameHost.Options.AnonymizePlayersOnCamerasDuringLights);
    }

    protected override void SetupTranslation()
    {

    }
}