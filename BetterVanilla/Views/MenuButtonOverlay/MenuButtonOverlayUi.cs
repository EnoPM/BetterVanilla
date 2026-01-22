using BetterVanilla.Ui;
using BetterVanilla.Ui.Core;
using BetterVanilla.Views.Menu;

namespace BetterVanilla.Views.MenuButtonOverlay;

public sealed partial class MenuButtonOverlayUi : BaseView
{
    public static MenuButtonOverlayUi? Instance { get; private set; }
    
    public MenuButtonOverlayUiViewModel? ViewModel { get; private set; }
    private MenuUi? Menu { get; set; }

    private void Start()
    {
        ViewModel = GetRequiredViewModel<MenuButtonOverlayUiViewModel>();
        Instance = this;
    }

    public void CloseMenu()
    {
        Destroy(Menu?.gameObject);
        Menu = null;
        ViewModel?.IsButtonInteractable = true;
    }

    partial void OnOkClicked()
    {
        if (Menu != null)
        {
            return;
        }
        Menu = UiManager.Instance.CreateView<MenuUi, MenuUiViewModel>(MenuUiViewModel.Instance);
        ViewModel?.IsButtonInteractable = false;
    }
}