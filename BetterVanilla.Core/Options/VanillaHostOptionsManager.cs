using BetterVanilla.Options;
using BetterVanilla.Options.Core;

namespace BetterVanilla.Core.Options;

public sealed class VanillaHostOptionsManager : OptionsManager<VanillaHostOptions>
{

    public VanillaHostOptionsManager(string filePath, double saveDelay = 3) : base(filePath, saveDelay)
    {
        
    }
}