using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Views;

public class MenuButtonOverlayViewModel : ViewModelBase
{
    public bool IsButtonInteractable
    {
        get;
        set => SetProperty(ref field, value);
    } = true;
}