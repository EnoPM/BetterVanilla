using System;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Ui;

public sealed class TextField : OptionBase
{
    public TMP_InputField input = null!;
    
    public event Action<string>? ValueChanged;
    
    public Options.Core.OptionTypes.StringOption? Option
    {
        get;
        set
        {
            if (value == field) return;
            BaseOption = field = value;
            if (field == null) return;
            input.SetTextWithoutNotify(field.Value);
        }
    }

    public void OnValueChanged(string value)
    {
        Option?.Value = value;
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }
}