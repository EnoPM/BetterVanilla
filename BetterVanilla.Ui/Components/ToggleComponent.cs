using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public class ToggleComponent : MonoBehaviour
{
    public Toggle toggle = null!;
    public Image background = null!;
    public Image checkmark = null!;
    public TextMeshProUGUI label = null!;
}