using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using TMPro;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public class NumberField : OptionBase
{
    public TextMeshProUGUI valueText = null!;
    public Button plusButton = null!;
    public Button minusButton = null!;

    public void OnPlusButtonClicked()
    {
        Ls.LogMessage("Plus Button clicked");
    }

    public void OnMinusButtonClicked()
    {
        Ls.LogMessage("Minus Button clicked");
    }
}