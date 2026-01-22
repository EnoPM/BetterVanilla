using System.Collections;
using System.Collections.Generic;
using BetterVanilla.Core;
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

    public string BecomeSponsorButtonText
    {
        get => field;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public string MenuName { get; } = ModData.Name;
    public string MenuVersion { get; } = $"v{ModData.Version}";

    public IEnumerable<string> AvailableLanguages
    {
        get;
    } = LocalizationManager.Instance.AvailableLanguages;

    public int SelectedLanguage
    {
        get;
        set
        {
            LocalizationManager.Instance.SetLanguageByIndex(value);
            SetProperty(ref field, value);
        }
    } = LocalizationManager.Instance.GetCurrentLanguageIndex();

    private MenuUiViewModel()
    {
        ReloadLocalizedTexts();
        LocalizationManager.Instance.LanguageChanged += ReloadLocalizedTexts;
    }

    private void ReloadLocalizedTexts()
    {
        BecomeSponsorButtonText = LocalizationManager.Instance[TranslationKey.Menu.BecomeSponsorButton];
    }
}