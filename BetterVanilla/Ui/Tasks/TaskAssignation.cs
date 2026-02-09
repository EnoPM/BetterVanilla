using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Tasks;

public sealed class TaskAssignation : LocalizationBehaviourBase
{
    public TextMeshProUGUI title = null!;
    public TaskAssignationForm form = null!;
    public Transform tableContainer = null!;
    public TaskAssignationEntry entryPrefab = null!;
    public TextMeshProUGUI playerLabel = null!;
    public TextMeshProUGUI commonTasksLabel = null!;
    public TextMeshProUGUI shortTasksLabel = null!;
    public TextMeshProUGUI longTasksLabel = null!;

    private void Start()
    {
        foreach (var assignation in TaskAssignationManager.Instance.Overrides)
        {
            Instantiate(entryPrefab, tableContainer).Override = assignation;
        }
    }

    public void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void OnFormSaveButtonClicked()
    {
        var player = form.Options?[form.playerDropdown.value];
        if (!player.HasValue) return;
        var data = new TaskAssignationManager.TaskAssignationOverride(player.Value.PlayerName,
            player.Value.FriendCode)
        {
            CommonTasks = form.commonTasksSelector.Value,
            ShortTasks = form.shortTasksSelector.Value,
            LongTasks = form.longTasksSelector.Value
        };
        TaskAssignationManager.Instance.Overrides.Add(data);
        TaskAssignationManager.Instance.Save();
        form.RefreshOptions();
        Instantiate(entryPrefab, tableContainer).Override = data;
    }

    protected override void SetupTranslation()
    {
        title.SetText(UiLocalization.TaskAssignationTitle);
        playerLabel.SetText(UiLocalization.PlayerLabel);
        commonTasksLabel.SetText(UiLocalization.CommonTasks);
        shortTasksLabel.SetText(UiLocalization.ShortTasks);
        longTasksLabel.SetText(UiLocalization.LongTasks);
    }
}