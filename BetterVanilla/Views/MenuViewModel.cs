using BetterVanilla.Ui.Helpers;

namespace BetterVanilla.Views;

public sealed class MenuViewModel : ViewModelBase
{
    public bool ShouldDisplayAdditionalTexts
    {
        get;
        set => SetProperty(ref field, value);
    } = false;
}