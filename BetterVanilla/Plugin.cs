using BetterVanilla.Components;
using BetterVanilla.Core;
using EnoUnityLoader.Attributes;
using EnoUnityLoader.Il2Cpp;

namespace BetterVanilla;

[ModInfos(ModData.Guid, ModData.Name, ModData.Version), ModProcess("Among Us")]
public sealed class Plugin : BasePlugin
{
    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<UnityThreadDispatcher>();
        AddComponent<FeatureCodeBehaviour>();
        AddComponent<BetterVanillaManager>();
        AddComponent<ModUpdaterBehaviour>();
        AddComponent<PlayerShieldBehaviour>();
    }
}