using System;
using BetterVanilla.Core;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Ui;

public sealed class DropdownField : OptionBase
{
    public TMP_Dropdown dropdown = null!;

    public event Action<int>? ValueChanged;
    
    public EnumOption? Option
    {
        get;
        set
        {
            if (value == field) return;
            BaseOption = field = value;
            if (field == null) return;
            RefreshOptions();
            dropdown.SetValueWithoutNotify(field.SelectedIndex);
        }
    }

    public void OnValueChanged(int value)
    {
        Option?.SelectedIndex = value;
        ValueChanged?.Invoke(value);
        TriggerValueUpdated();
    }

    private void RefreshOptions()
    {
        if (Option == null) return;
        var options = new Il2CppSystem.Collections.Generic.List<string>();
        foreach (var choice in Option.Choices)
        {
            options.Add(choice.Label);
        }
        var selectedIndex = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(selectedIndex);
    }

    protected override void SetupTranslation()
    {
        base.SetupTranslation();
        RefreshOptions();
    }
}