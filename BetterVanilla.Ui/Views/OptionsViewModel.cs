using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Ui.Views;

/// <summary>
/// ViewModel for the OptionsView.
/// </summary>
public sealed class OptionsViewModel : ViewModelBase
{
    private bool _isFeatureEnabled;
    private float _volume = 0.5f;
    private string _username = string.Empty;

    public bool IsFeatureEnabled
    {
        get => _isFeatureEnabled;
        set => SetProperty(ref _isFeatureEnabled, value);
    }

    public float Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, value);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public void SaveSettings()
    {
        // Save settings logic here
    }

    public void ResetToDefaults()
    {
        IsFeatureEnabled = false;
        Volume = 0.5f;
        Username = string.Empty;
    }
}
