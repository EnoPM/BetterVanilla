using BetterVanilla.Options;

namespace BetterVanilla.Core.Extensions;

public static class OptionBehaviourExtensions
{
    extension(OptionBehaviour optionBehaviour)
    {
        public bool CustomUpdateValue()
        {
            var customOption = HostOptions.Default.FindOptionByBehaviour(optionBehaviour);
            return customOption == null;
        }
    }
}