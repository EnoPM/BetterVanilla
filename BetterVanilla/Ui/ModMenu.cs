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
        TabBase[] tabs = [homeTab, hostTab, sponsorTab, userTab, cosmeticsTab];
        foreach (var tab in tabs)
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
    }
}