using System;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui.Base;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class ToggleField : OptionBase
{
    public Toggle toggle = null!;
    
    public event Action<bool>? ValueChanged; 
    
    public BoolOption? Option
    {
        get;
        set
        {
            if (value == field) return;
            BaseOption = field = value;
            if (field == null) return;
            toggle.SetIsOnWithoutNotify(field.Value);
        }
    }

    protected override void SetInteractable(bool isInteractable)
    {
        base.SetInteractable(isInteractable);
        toggle.interactable = isInteractable;
    }

    public void OnValueChanged(bool value)
    {
        Option?.Value = value;
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }
}