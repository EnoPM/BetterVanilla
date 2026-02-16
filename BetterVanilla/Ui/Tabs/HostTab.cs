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
        this.AddOption(ModOptions.GameHost.Options.AllowTeamPreference);
        this.AddOption(ModOptions.GameHost.Options.AllowDeadVoteDisplay);
        this.AddOption(ModOptions.GameHost.Options.ProtectFirstKilledPlayer);
        this.AddOption(ModOptions.GameHost.Options.ProtectionDuration);
        

        this.AddCategoryTitle(() => UiLocalization.TasksCategoryTitle);
        this.AddOption(ModOptions.GameHost.Options.DefineCommonTasksAsNonCommon);
        this.AddOption(ModOptions.GameHost.Options.RandomizeFixWiringTaskOrder);
        this.AddOption(ModOptions.GameHost.Options.RandomizeUploadTaskLocation);
        
        this.AddCategoryTitle(() => UiLocalization.MapCategoryTitle);
        this.AddOption(ModOptions.GameHost.Options.HideDeadPlayerPets);
        this.AddOption(ModOptions.GameHost.Options.BetterPolus);
        this.AddOption(ModOptions.GameHost.Options.PolusReactorCountdown);
        this.AddOption(ModOptions.GameHost.Options.AnonymizePlayersOnCamerasDuringLights);
        
        this.AddCategoryTitle(() => UiLocalization.MeetingsCategoryTitle);
        this.AddOption(ModOptions.GameHost.Options.RandomizePlayerOrderInMeetings);
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