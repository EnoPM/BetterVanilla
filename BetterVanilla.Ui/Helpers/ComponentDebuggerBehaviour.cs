using BetterVanilla.Ui.Views;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Helpers;

public sealed class ComponentDebuggerBehaviour : MonoBehaviour
{
    private void Update()
    {
        OptionsView? view = null;
        if (Input.GetKeyDown(KeyCode.T))
        {
            AssetBundleManager.Instance.LoadFromEmbeddedResource("BetterVanilla.Ui.Assets.ui.bundle");

            view = UiManager.Instance.CreateView<OptionsView, OptionsViewModel>(new OptionsViewModel
            {
                IsFeatureEnabled = true,
                Volume = 0.75f,
                Username = "Player"
            });

            if (HudManager.InstanceExists && HudManager.Instance.UseButton != null)
            {
                UiDebugger.LogFullHierarchyAnalysis(HudManager.Instance.UseButton.gameObject);
            }
            
            // UiDebugger.LogFullHierarchyAnalysis(view.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Plugin.Instance.Log.LogMessage($"P key pressed! {view != null}");
            if (view != null)
            {
                UiDebugger.LogFullHierarchyAnalysis(view.gameObject);
            }
            //FindComponentByTextMeshProValue("Could not connect to your Among Us Account.");
        }
    }

    private void FindComponentByTextMeshProValue(string valueToFind)
    {
        var objects = FindObjectsOfType<TextMeshPro>();
        foreach (var tmp in objects)
        {
            if (tmp.text.Contains(valueToFind, System.StringComparison.InvariantCultureIgnoreCase))
            {
                System.Console.WriteLine("Component found!");
                UiDebugger.LogFullHierarchyAnalysis(tmp.gameObject);
            }
        }
    }
}