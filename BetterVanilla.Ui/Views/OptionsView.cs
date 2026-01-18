using System;
using BetterVanilla.Ui.Core;

namespace BetterVanilla.Ui.Views;

/// <summary>
/// User code for OptionsView.
/// The partial class is completed by the generated OptionsView.g.cs file.
/// </summary>
public partial class OptionsView : BaseView
{
    public event Action? SettingsSaved;
    public event Action? Cancelled;
    public event Action? Closed;

    /// <summary>
    /// Gets the typed ViewModel.
    /// </summary>
    private OptionsViewModel ViewModel => GetRequiredViewModel<OptionsViewModel>();

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
            Plugin.Instance.Log.LogMessage($"OptionsView bound to ViewModel: Feature={vm.IsFeatureEnabled}, Volume={vm.Volume}");
        }
    }

    // Event handlers (partial methods implemented here)

    partial void OnFeatureToggleChanged(bool value)
    {
        Plugin.Instance.Log.LogMessage($"Feature toggle changed to: {value}");
    }

    partial void OnVolumeChanged(float value)
    {
        Plugin.Instance.Log.LogMessage($"Volume changed to: {value:F2}");
    }

    partial void OnUsernameChanged(string value)
    {
        Plugin.Instance.Log.LogMessage($"Username changed to: {value}");
    }

    partial void OnDifficultyChanged(int value)
    {
        Plugin.Instance.Log.LogMessage($"Difficulty changed to: {value}");
    }

    partial void OnSaveClicked()
    {
        ViewModel.SaveSettings();
        SettingsSaved?.Invoke();
        Plugin.Instance.Log.LogMessage("Settings saved!");
    }

    partial void OnCancelClicked()
    {
        ViewModel.ResetToDefaults();
        Cancelled?.Invoke();
        Plugin.Instance.Log.LogMessage("Settings cancelled.");
    }

    partial void OnCloseClicked()
    {
        Plugin.Instance.Log.LogMessage("Close button clicked, destroying view.");
        Closed?.Invoke();
        Destroy(gameObject);
    }
}
