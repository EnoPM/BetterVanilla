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

    private void Start()
    {
        localizationDropdown.SetValueWithoutNotify(LocalizationManager.CurrentLanguageIndex);

        this.AddOption(ModOptions.User.Options.TaskStartColor);
        this.AddOption(ModOptions.User.Options.TaskEndColor);
        this.AddOption(ModOptions.User.Options.DisplayBetterVanillaVersion);
    }

    protected override void SetupTranslation()
    {
        base.SetupTranslation();
        languageDropdownLabel.SetText(ModOptions.User.Options.Language.Label);
        becomeSponsorText.SetText(UiLocalization.BecomeSponsorText);
        becomeSponsorButtonText.SetText(UiLocalization.BecomeSponsorButton);
    }

    public void OnBecomeSponsorButtonClicked()
    {
        Ls.LogMessage($"BecomeSponsor button clicked");
    }
}