using BetterVanilla.Core.Extensions;
using BetterVanilla.Ui.Base;

namespace BetterVanilla.Ui.Tabs;

public sealed class HostTab : TabBase
{
    private ToggleField? ProtectFirstKilledPlayer { get; set; }
    private NumberField? ProtectionDuration { get; set; }
    
    private void Start()
    {
        this.AddOption(ModOptions.GameHost.Options.AllowDeadVoteDisplay);
        this.AddOption(ModOptions.GameHost.Options.AllowTeamPreference);
        this.AddOption(ModOptions.GameHost.Options.HideDeadPlayerPets);
        this.AddOption(ModOptions.GameHost.Options.PolusReactorCountdown);
        ProtectFirstKilledPlayer = this.AddOption(ModOptions.GameHost.Options.ProtectFirstKilledPlayer);
        ProtectionDuration = this.AddOption(ModOptions.GameHost.Options.ProtectionDuration);
        this.AddOption(ModOptions.GameHost.Options.DefineCommonTasksAsNonCommon);
        this.AddOption(ModOptions.GameHost.Options.BetterPolus);
        this.AddOption(ModOptions.GameHost.Options.RandomizeFixWiringTaskOrder);
        this.AddOption(ModOptions.GameHost.Options.RandomizeUploadTaskLocation);
        this.AddOption(ModOptions.GameHost.Options.RandomizePlayerOrderInMeetings);
        this.AddOption(ModOptions.GameHost.Options.AnonymizePlayersOnCamerasDuringLights);

        ModOptions.GameHost.Options.ProtectFirstKilledPlayer.ValueChanged += UpdateProtectionDurationState;
        
        UpdateProtectionDurationState();
    }

    private void UpdateProtectionDurationState()
    {
        ProtectionDuration?.gameObject.SetActive(ModOptions.GameHost.Options.ProtectFirstKilledPlayer.Value);
    }

    protected override void SetupTranslation()
    {
        
    }
}