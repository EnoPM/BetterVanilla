using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Ui.Base;
using BetterVanilla.Ui.Tasks;
using TMPro;

namespace BetterVanilla.Ui.Tabs;

public sealed class HostTab : TabBase
{
    public TextMeshProUGUI manageTaskAssignationButtonText = null!;
    public TaskAssignation taskAssignation = null!;
    
    private void Start()
    {
        this.AddCategoryTitle(() => UiLocalization.PlayersCategoryTitle);
        this.AddOption(ModOptions.VanillaHost.Options.PlayerSpeed);
        this.AddOption(ModOptions.GameHost.Options.AllowTeamPreference);
        this.AddOption(ModOptions.GameHost.Options.AllowDeadVoteDisplay);
        this.AddOption(ModOptions.GameHost.Options.ProtectFirstKilledPlayer);
        this.AddOption(ModOptions.GameHost.Options.ProtectionDuration);
        
        this.AddCategoryTitle(() => UiLocalization.ImpostorsCategoryTitle);
        this.AddOption(ModOptions.VanillaHost.Options.ImpostorsCount);
        this.AddOption(ModOptions.VanillaHost.Options.KillCooldown);
        this.AddOption(ModOptions.VanillaHost.Options.ImpostorVision);
        this.AddOption(ModOptions.VanillaHost.Options.KillDistance);

        this.AddCategoryTitle(() => UiLocalization.CrewmatesCategoryTitle);
        this.AddOption(ModOptions.VanillaHost.Options.CrewmateVision);
        
        this.AddCategoryTitle(() => UiLocalization.MeetingsCategoryTitle);
        this.AddOption(ModOptions.VanillaHost.Options.NumberOfMeetingsPerPlayer);
        this.AddOption(ModOptions.VanillaHost.Options.EmergencyMeetingCooldown);
        this.AddOption(ModOptions.VanillaHost.Options.MeetingDiscussionTime);
        this.AddOption(ModOptions.VanillaHost.Options.MeetingVotingTime);
        this.AddOption(ModOptions.VanillaHost.Options.AnonymousVotes);
        this.AddOption(ModOptions.VanillaHost.Options.ConfirmEject);
        this.AddOption(ModOptions.GameHost.Options.RandomizePlayerOrderInMeetings);
        
        this.AddCategoryTitle(() => UiLocalization.MapCategoryTitle);
        this.AddOption(ModOptions.GameHost.Options.HideDeadPlayerPets);
        this.AddOption(ModOptions.GameHost.Options.BetterPolus);
        this.AddOption(ModOptions.GameHost.Options.PolusReactorCountdown);
        this.AddOption(ModOptions.GameHost.Options.AnonymizePlayersOnCamerasDuringLights);
        
        this.AddCategoryTitle(() => UiLocalization.TasksCategoryTitle);
        this.AddOption(ModOptions.VanillaHost.Options.TaskBarUpdates);
        this.AddOption(ModOptions.VanillaHost.Options.CommonTasks);
        this.AddOption(ModOptions.VanillaHost.Options.LongTasks);
        this.AddOption(ModOptions.VanillaHost.Options.ShortTasks);
        this.AddOption(ModOptions.VanillaHost.Options.VisualTasks);
        this.AddOption(ModOptions.GameHost.Options.DefineCommonTasksAsNonCommon);
        this.AddOption(ModOptions.GameHost.Options.RandomizeFixWiringTaskOrder);
        this.AddOption(ModOptions.GameHost.Options.RandomizeUploadTaskLocation);
    }

    public void OnManagedTaskAssignmentButtonClicked()
    {
        taskAssignation.gameObject.SetActive(true);
    }

    protected override void SetupTranslation()
    {
        base.SetupTranslation();
        manageTaskAssignationButtonText.SetText(UiLocalization.TaskAssignationButton);
    }
}