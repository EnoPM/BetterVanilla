using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class DropdownTemplateComponent : MonoBehaviour
{
    public Image background = null!;
    public ScrollRect scrollRect = null!;
    public Image viewport = null!;
    public RectTransform content = null!;
    public Toggle itemToggle = null!;
    public Image itemBackground = null!;
    public Image itemCheckmark = null!;
    public TextMeshProUGUI itemLabel = null!;
    public ScrollbarComponent scrollbarComponent = null!;
}