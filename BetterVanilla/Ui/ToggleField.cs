using System;
using BetterVanilla.Ui.Base;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class ToggleField : OptionBase
{
    public Toggle toggle = null!;
    
    public event Action<bool> ValueChanged; 
    
    public void OnValueChanged(bool value)
    {
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }
}