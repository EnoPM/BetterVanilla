using System;
using BetterVanilla.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Tasks;

public sealed class TaskAssignationEntry : MonoBehaviour
{
    public TextMeshProUGUI playerName = null!;
    public TextMeshProUGUI friendCode = null!;
    public TaskCountEditor commonTasks = null!;
    public TaskCountEditor longTasks = null!;
    public TaskCountEditor shortTasks = null!;
    public Button deleteButton = null!;

    public TaskAssignationManager.TaskAssignationOverride? Override
    {
        get;
        set
        {
            if (value == null) return;
            field = value;
            playerName.SetText(field.PlayerName);
            friendCode.SetText(field.FriendCode);
            commonTasks.Value = field.CommonTasks;
            longTasks.Value = field.LongTasks;
            shortTasks.Value = field.ShortTasks;
        }
    }

    private void Awake()
    {
        commonTasks.ValueChanged += OnTaskValueChanged;
        longTasks.ValueChanged += OnTaskValueChanged;
        shortTasks.ValueChanged += OnTaskValueChanged;
    }

    private void OnTaskValueChanged()
    {
        if (Override == null) return;
        var hasUpdate = false;
        if (commonTasks.Value != Override.CommonTasks)
        {
            Override.CommonTasks = commonTasks.Value;
            hasUpdate = true;
        }
        
        if (shortTasks.Value != Override.ShortTasks)
        {
            Override.ShortTasks = shortTasks.Value;
            hasUpdate = true;
        }

        if (longTasks.Value != Override.LongTasks)
        {
            Override.LongTasks = longTasks.Value;
            hasUpdate = true;
        }

        if (hasUpdate)
        {
            TaskAssignationManager.Instance.Save();
        }
    }

    public void OnDeleteButtonClicked()
    {
        if (Override == null) return;
        TaskAssignationManager.Instance.Overrides.Remove(Override);
        TaskAssignationManager.Instance.Save();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Override == null) return;
        commonTasks.ValueChanged -= OnTaskValueChanged;
        longTasks.ValueChanged -= OnTaskValueChanged;
        shortTasks.ValueChanged -= OnTaskValueChanged;
    }
}