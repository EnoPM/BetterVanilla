using System.Collections.Generic;
using BetterVanilla.Localization;
using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Views.Menu;

public sealed class MenuUiViewModel : ViewModelBase
{
    public static MenuUiViewModel Instance { get; } = new();
    
    public bool ShouldDisplayAdditionalTexts
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public string MenuVersion { get; } = $"v{ModData.Version}";

    public IEnumerable<string> AvailableLanguages => LocalizationManager.AvailableLanguageNames;

    public int SelectedLanguage
    {
        get => LocalizationManager.CurrentLanguageIndex;
        set => LocalizationManager.CurrentLanguage = LocalizationManager.AvailableLanguages[value];
    }
}