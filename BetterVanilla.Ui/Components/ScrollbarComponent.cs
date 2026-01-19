using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class ScrollbarComponent : MonoBehaviour
{
    public Scrollbar scrollbar = null!;
    public Image background = null!;
    public RectTransform slidingArea = null!;
    public Image handle = null!;
}