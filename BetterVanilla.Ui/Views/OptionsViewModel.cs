using System.Collections.Generic;
using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Ui.Views;

/// <summary>
/// ViewModel for the OptionsView.
/// </summary>
public sealed class OptionsViewModel : ViewModelBase
{

    public bool IsFeatureEnabled
    {
        get;
        set => SetProperty(ref field, value);
    }

    public float Volume
    {
        get;
        set => SetProperty(ref field, value);
    } = 0.5f;

    public string Username
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;
    
    public IEnumerable<string> DifficultyOptions { get; } = ["Facile", "Normal", "Difficile"];

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
