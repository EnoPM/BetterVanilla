using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class ModMenuButtons : LocalizationBehaviourBase
{
    public Button openMenuButton = null!;
    public Button zoomInButton = null!;
    public Button zoomOutButton = null!;
    public Button finishTasksButton = null!;
    
    protected override void SetupTranslation()
    {
        
    }

    public void OnZoomInButtonClicked()
    {
        Ls.LogMessage($"Zoom in button clicked");
    }

    public void OnZoomOutButtonClicked()
    {
        Ls.LogMessage($"Zoom out button clicked");
    }

    public void OnFinishTasksButtonClicked()
    {
        Ls.LogMessage($"Finish tasks button clicked");
    }
}