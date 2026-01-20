using System;
using BetterVanilla.Core;
using BetterVanilla.Ui.Core;

namespace BetterVanilla.Views;

public sealed partial class Menu : BaseView
{
    private MenuViewModel? ViewModel { get; set; }

    private void Start()
    {
        ViewModel = GetRequiredViewModel<MenuViewModel>();
    }

    partial void OnCloseClicked()
    {
        MenuButtonOverlay.Instance?.CloseMenu();
    }

    partial void OnBecomeSponsorButtonClick()
    {
        if (ViewModel == null) return;
        ViewModel.ShouldDisplayAdditionalTexts = !ViewModel.ShouldDisplayAdditionalTexts;
        Ls.LogMessage($"Clicked on become sponsor button");
    }
}