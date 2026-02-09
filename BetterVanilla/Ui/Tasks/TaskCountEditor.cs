using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Tasks;

public class TaskCountEditor : MonoBehaviour
{
    public TextMeshProUGUI valueText = null!;
    public Button minusButton = null!;
    public Button plusButton = null!;

    public int Value
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            ValueChanged?.Invoke();
            RefreshInteractableStates();
            valueText.SetText(field.ToString());
        }
    }

    public int Max
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            RefreshInteractableStates();
        }
    }

    public event Action? ValueChanged;

    public virtual void OnMinusButtonClicked()
    {
        var newValue = Value - 1;
        if (newValue < 0)
        {
            newValue = 0;
        }

        if (newValue > Max)
        {
            newValue = Max;
        }
        
        Value = newValue;
    }

    public virtual void OnPlusButtonClicked()
    {
        var newValue = Value + 1;
        if (newValue < 0)
        {
            newValue = 0;
        }

        if (newValue > Max)
        {
            newValue = Max;
        }
        
        Value = newValue;
    }

    private void RefreshInteractableStates()
    {
        minusButton.interactable = Value > 0;
        plusButton.interactable = Value < Max;
    }
}