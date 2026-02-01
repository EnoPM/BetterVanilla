using System;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Ui.Base;

public abstract class OptionBase : LocalizationBehaviourBase
{
    public TextMeshProUGUI label = null!;
    public GameObject infos = null!;
    public TextMeshProUGUI infosText = null!;

    public event Action? ValueUpdated;

    public Options.Core.OptionBase? BaseOption
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            SetupTranslation();
        }
    }

    protected void TriggerValueUpdated() => ValueUpdated?.Invoke();

    protected override void SetupTranslation()
    {
        if (BaseOption == null || !enabled) return;
        label.SetText(BaseOption.Label);
        infosText.SetText(BaseOption.Description);
    }
}