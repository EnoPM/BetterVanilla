using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui;

public sealed class TabHeader : MonoBehaviour
{
    public Button button = null!;
    public Image icon = null!;

    private void OnEnable()
    {
        button.interactable = true;
        icon.color = Color.white;
    }

    private void OnDisable()
    {
        button.interactable = false;
        icon.color = Palette.DisabledClear;
    }
}