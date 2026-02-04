using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Ui.Base;
using BetterVanilla.Ui.Tabs;
using TMPro;

namespace BetterVanilla.Ui;

public sealed class ModMenu : LocalizationBehaviourBase
{
    public TextMeshProUGUI titleText = null!;
    public TextMeshProUGUI versionText = null!;
    public HomeTab homeTab = null!;
    public HostTab hostTab = null!;
    public SponsorTab sponsorTab = null!;
    public UserTab userTab = null!;
    public CosmeticsTab cosmeticsTab = null!;

    private TabBase[] AllTabs => [homeTab, hostTab, sponsorTab, userTab, cosmeticsTab];

    private void Awake()
    {
        versionText.SetText($"v{ModData.Version}");
    }

    private void Start()
    {
        SelectHomeTab();
    }
    
    public void SelectHomeTab() => SelectTab(homeTab);
    public void SelectHostTab() => SelectTab(hostTab);
    public void SelectSponsorTab() => SelectTab(sponsorTab);
    public void SelectUserTab() => SelectTab(userTab);
    public void SelectCosmeticsTab() => SelectTab(cosmeticsTab);

    private void SelectTab(TabBase selectedTab)
    {
        foreach (var tab in AllTabs)
        {
            if (tab == selectedTab)
            {
                tab.gameObject.SetActive(true);
                tab.header.enabled = false;
                continue;
            }
            tab.gameObject.SetActive(false);
            tab.header.enabled = true;
        }
    }

    protected override void SetupTranslation()
    {
        titleText.SetText(UiLocalization.MenuTitle);
        if (homeTab.IsHeaderActive)
        {
            homeTab.header.title.SetText(UiLocalization.HomeTabTitle);
        }
        if (hostTab.IsHeaderActive)
        {
            hostTab.header.title.SetText(UiLocalization.HostTabTitle);
        }
        if (sponsorTab.IsHeaderActive)
        {
            sponsorTab.header.title.SetText(UiLocalization.SponsorTabTitle);
        }
        if (userTab.IsHeaderActive)
        {
            userTab.header.title.SetText(UiLocalization.GameTabTitle);
        }
        if (cosmeticsTab.IsHeaderActive)
        {
            cosmeticsTab.header.title.SetText(UiLocalization.OutfitsTabTitle);
        }
    }
}