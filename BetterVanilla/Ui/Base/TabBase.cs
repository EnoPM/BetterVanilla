using UnityEngine;

namespace BetterVanilla.Ui.Base;

public abstract class TabBase : LocalizationBehaviourBase
{
    public TabHeader header = null!;
    public Transform container = null!;
}