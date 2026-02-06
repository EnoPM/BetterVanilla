using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class NumberOptionExtensions
{
    private const int IncrementMultiplier = 10;
    
    extension(NumberOption option)
    {
        public void BetterIncrease()
        {
            if (Mathf.Approximately(option.Value, option.ValidRange.max)) return;
            var multiplier = option.ValidRange.max - option.ValidRange.min >= IncrementMultiplier * option.Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
            option.Value = option.ValidRange.Clamp(option.Value + option.Increment * multiplier);
            option.UpdateValue();
            option.OnValueChanged.Invoke(option);
            option.AdjustButtonsActiveState();
        }

        public void BetterDecrease()
        {
            if (Mathf.Approximately(option.Value, option.ValidRange.min)) return;
            var multiplier = option.ValidRange.max - option.ValidRange.min >= IncrementMultiplier * option.Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
            option.Value = option.ValidRange.Clamp(option.Value - option.Increment * multiplier);
            option.UpdateValue();
            option.OnValueChanged.Invoke(option);
            option.AdjustButtonsActiveState();
        }
    }
}