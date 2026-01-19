using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class DropdownComponent : MonoBehaviour
{
    public TMP_Dropdown dropdown = null!;
    public TextMeshProUGUI label = null!;
    public Image arrow = null!;
    public DropdownTemplateComponent template = null!;
}