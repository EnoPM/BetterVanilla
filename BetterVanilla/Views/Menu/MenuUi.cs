using BetterVanilla.Core;
using BetterVanilla.Ui.Core;
using BetterVanilla.Views.MenuButtonOverlay;

namespace BetterVanilla.Views.Menu;

public sealed partial class MenuUi : BaseView
{
    private MenuUiViewModel? ViewModel { get; set; }

    private void Start()
    {
        ViewModel = GetRequiredViewModel<MenuUiViewModel>();
    }

    partial void OnCloseClicked()
    {
        MenuButtonOverlayUi.Instance?.CloseMenu();
    }

    partial void OnBecomeSponsorButtonClick()
    {
        if (ViewModel == null) return;
        ViewModel.ShouldDisplayAdditionalTexts = !ViewModel.ShouldDisplayAdditionalTexts;
        Ls.LogMessage($"Clicked on become sponsor button");
    }
}