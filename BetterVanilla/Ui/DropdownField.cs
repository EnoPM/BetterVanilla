using System;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Ui;

public sealed class DropdownField : OptionBase
{
    public TMP_Dropdown dropdown = null!;

    public event Action<int>? ValueChanged;

    public void OnSelectedIndexChanged(int value)
    {
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }
}