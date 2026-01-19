using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Ui.Components;

public sealed class InputFieldComponent : MonoBehaviour
{
    public TMP_InputField inputField = null!;
    public Image background = null!;
    public Shadow shadow = null!;
    public RectTransform textArea = null!;
    public TextMeshProUGUI text = null!;
    public TextMeshProUGUI placeholder = null!;
}