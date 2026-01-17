using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;

namespace BetterVanilla.Ui;

[ModInfos(ModData.Guid, ModData.Name, ModData.Version), ModProcess("Among Us")]
public sealed class Plugin : BasePlugin
{
    public override void Load()
    {
        // Initialize the AssetBundleManager with the UI bundle
        // This would typically load the bundle containing UI prefabs
        // AssetBundleManager.Instance.LoadFromEmbeddedResource("BetterVanilla.Ui.Assets.ui.bundle");

        // Example of creating a view with ViewModel:
        // var optionsView = new GameObject("OptionsView").AddComponent<Views.OptionsView>();
        // optionsView.DataContext = new Views.OptionsViewModel
        // {
        //     IsFeatureEnabled = true,
        //     Volume = 0.75f,
        //     Username = "Player"
        // };
    }
}