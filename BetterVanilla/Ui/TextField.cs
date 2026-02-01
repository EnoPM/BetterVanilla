using System;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Ui;

public sealed class TextField : OptionBase
{
    public TMP_InputField input = null!;
    
    public event Action<string>? ValueChanged;

    public void OnValueChanged(string value)
    {
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }
}