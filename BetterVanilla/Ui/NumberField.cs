using System;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class NumberField : OptionBase
{
    public TextMeshProUGUI valueText = null!;
    public Button plusButton = null!;
    public Button minusButton = null!;
    
    public event Action<float>? ValueChanged;
    
    public FloatOption? Option
    {
        get;
        set
        {
            if (value == field) return;
            BaseOption = field = value;
            if (field == null) return;
            SetValueWithoutNotify(field.Value);
        }
    }

    public void OnPlusButtonClicked()
    {
        if (Option == null) return;
        SetValue(Option.Value + (Option.Step ?? 1f));
    }

    public void OnMinusButtonClicked()
    {
        if (Option == null) return;
        SetValue(Option.Value - (Option.Step ?? 1f));
    }

    public void SetValueWithoutNotify(float value)
    {
        if (Option == null) return;
        if (!Mathf.Approximately(value, Option.Value))
        {
            Option.Value = Mathf.Clamp(value, Option.Min ?? float.MinValue, Option.Max ?? float.MaxValue);
        }
        valueText.SetText($"{Option.Prefix}{Option.Value}{Option.Suffix}");
    }

    protected override void SetInteractable(bool isInteractable)
    {
        base.SetInteractable(isInteractable);
        plusButton.interactable = isInteractable;
        minusButton.interactable = isInteractable;
    }

    public void SetValue(float value)
    {
        if (Option == null || Mathf.Approximately(value, Option.Value)) return;
        SetValueWithoutNotify(value);
        ValueChanged?.Invoke(Option.Value);
        TriggerValueUpdated();
    }
}