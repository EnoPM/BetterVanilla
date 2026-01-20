using BetterVanilla.Ui;
using BetterVanilla.Ui.Core;

namespace BetterVanilla.Views;

public sealed partial class MenuButtonOverlay : BaseView
{
    public static MenuButtonOverlay? Instance { get; private set; }
    
    public MenuButtonOverlayViewModel? ViewModel { get; private set; }
    private Menu? Menu { get; set; }

    private void Start()
    {
        ViewModel = GetRequiredViewModel<MenuButtonOverlayViewModel>();
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
        Menu = UiManager.Instance.CreateView<Menu, MenuViewModel>();
        ViewModel?.IsButtonInteractable = false;
    }

    private void Update()
    {
        if (ViewModel == null) return;
        //ViewModel.IsButtonInteractable = PlayerControl.LocalPlayer != null;
    }
}