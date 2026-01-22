using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Views.MenuButtonOverlay;

public class MenuButtonOverlayUiViewModel : ViewModelBase
{
    public bool IsButtonInteractable
    {
        get;
        set => SetProperty(ref field, value);
    } = true;
}