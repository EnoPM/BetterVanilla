using System;
using BetterVanilla.Ui.Core;
using UnityEngine;

namespace BetterVanilla.Ui.Views;

/// <summary>
/// User code for OptionsView.
/// The partial class is completed by the generated OptionsView.g.cs file.
/// </summary>
public partial class OptionsView : BaseView<OptionsViewModel>
{
    public event Action? SettingsSaved;
    public event Action? Cancelled;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Additional initialization
        titleText.Text = "Game Options";
    }

    protected override void OnDataContextChanged(object? newDataContext)
    {
        base.OnDataContextChanged(newDataContext);

        // Update UI when ViewModel changes
        if (newDataContext is OptionsViewModel vm)
        {
            Debug.Log($"OptionsView bound to ViewModel: Feature={vm.IsFeatureEnabled}, Volume={vm.Volume}");
        }
    }

    // Event handlers (partial methods implemented here)

    partial void OnFeatureToggleChanged(bool value)
    {
        Debug.Log($"Feature toggle changed to: {value}");
    }

    partial void OnVolumeChanged(float value)
    {
        Debug.Log($"Volume changed to: {value:F2}");
    }

    partial void OnUsernameChanged(string value)
    {
        Debug.Log($"Username changed to: {value}");
    }

    partial void OnSaveClicked()
    {
        ViewModel.SaveSettings();
        SettingsSaved?.Invoke();
        Debug.Log("Settings saved!");
    }

    partial void OnCancelClicked()
    {
        ViewModel.ResetToDefaults();
        Cancelled?.Invoke();
        Debug.Log("Settings cancelled.");
    }
}
