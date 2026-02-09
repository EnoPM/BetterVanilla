using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Extensions;
using BetterVanilla.Ui.Base;
using TMPro;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Tasks;

public class TaskAssignationForm : LocalizationBehaviourBase
{
    public TMP_Dropdown playerDropdown = null!;
    public Button saveButton = null!;
    public TextMeshProUGUI saveButtonText = null!;
    public TextMeshProUGUI playerLabel = null!;
    public TaskCountSelector commonTasksSelector = null!;
    public TaskCountSelector shortTasksSelector = null!;
    public TaskCountSelector longTasksSelector = null!;
    
    public List<(string FriendCode, string PlayerName)>? Options { get; set; }

    protected override void SetupTranslation()
    {
        playerLabel.SetText(UiLocalization.PlayerLabel);
        saveButtonText.SetText(UiLocalization.SaveTaskAssignationButton);
        
        if (commonTasksSelector != null && commonTasksSelector.label != null)
        {
            commonTasksSelector.label.SetText(UiLocalization.CommonTasks);
        }
        
        if (shortTasksSelector != null && shortTasksSelector.label != null)
        {
            shortTasksSelector.label.SetText(UiLocalization.ShortTasks);
        }
        
        if (longTasksSelector != null && longTasksSelector.label != null)
        {
            longTasksSelector.label.SetText(UiLocalization.LongTasks);
        }
    }

    public void RefreshOptions()
    {
        playerDropdown.ClearOptions();
        Options = [];
        foreach (var player in BetterVanillaManager.Instance.AllPlayers)
        {
            if (string.IsNullOrEmpty(player.FriendCode)) continue;
            if (TaskAssignationManager.Instance.Overrides.Any(x => x.FriendCode == player.FriendCode)) continue;
            Options.Add((player.FriendCode, player.Player.Data.PlayerName));
        }
        playerDropdown.SetValue(0, false);
        playerDropdown.AddOptions(Options.Select(o => o.PlayerName).ToIl2CppList());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshOptions();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}