using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Localization;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Ui.Tabs;

public sealed class HomeTab : TabBase
{
    public TextMeshProUGUI languageDropdownLabel = null!;
    public TextMeshProUGUI becomeSponsorText = null!;
    public TextMeshProUGUI becomeSponsorButtonText = null!;
    public TMP_Dropdown localizationDropdown = null!;

    private void Awake()
    {
        localizationDropdown.SetValueWithoutNotify(LocalizationManager.CurrentLanguageIndex);
    }

    private void Start()
    {
        ModOptions.User.AddToTab(this, ModOptions.User.Options.TaskStartColor);
        ModOptions.User.AddToTab(this, ModOptions.User.Options.TaskEndColor);
        ModOptions.GameLocal.AddToTab(this, ModOptions.GameLocal.Options.TeamPreference);
    }

    protected override void SetupTranslation()
    {
        languageDropdownLabel.SetText(ModOptions.User.Options.Language.Label);
        becomeSponsorText.SetText(UiLocalization.BecomeSponsorText);
        becomeSponsorButtonText.SetText(UiLocalization.BecomeSponsorButton);
    }

    public void OnBecomeSponsorButtonClicked()
    {
        Ls.LogMessage($"BecomeSponsor button clicked");
    }
}