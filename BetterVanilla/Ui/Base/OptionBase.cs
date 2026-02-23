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
            if (field != null)
            {
                field.EnabledChanged -= OnEnabledChanged;
                field.VisibleChanged -= OnVisibleChanged;
            }
            field = value;
            if (value != null)
            {
                value.EnabledChanged += OnEnabledChanged;
                value.VisibleChanged += OnVisibleChanged;
                OnEnabledChanged();
                OnVisibleChanged();
            }
            SetupTranslation();
        }
    }

    private void OnEnabledChanged()
    {
        if (BaseOption == null) return;
        SetInteractable(BaseOption.IsEnabled);
    }

    private void OnVisibleChanged()
    {
        if (BaseOption == null) return;
        SetActive(BaseOption.IsVisible);
    }

    protected virtual void SetInteractable(bool isInteractable)
    {
    }

    public abstract void UpdateFromOption();

    protected virtual void SetActive(bool isActive) => gameObject.SetActive(isActive);

    protected void TriggerValueUpdated() => ValueUpdated?.Invoke();

    protected void OnDestroy()
    {
        if (BaseOption == null) return;
        BaseOption.EnabledChanged -= OnEnabledChanged;
        BaseOption.VisibleChanged -= OnVisibleChanged;
    }

    protected override void SetupTranslation()
    {
        if (BaseOption == null || !enabled) return;
        label.SetText(BaseOption.Label);
        infosText.SetText(BaseOption.Description);
    }
}