using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class ScrollViewComponent : MonoBehaviour
{
    public Image background = null!;
    public ScrollRect scrollRect = null!;
    public Image viewport = null!;
    public RectTransform content = null!;
    public ScrollbarComponent horizontalScrollbar = null!;
    public ScrollbarComponent verticalScrollbar = null!;
}