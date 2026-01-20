using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class SliderComponent : MonoBehaviour
{
    public Slider slider = null!;
    public Image background = null!;
    public Image fill = null!;
    public Image handle = null!;
}