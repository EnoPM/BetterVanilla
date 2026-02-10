using BetterVanilla.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Ui.Base;

public abstract class TabBase : LocalizationBehaviourBase
{
    public TabHeader header = null!;
    public Transform container = null!;

    protected override void SetupTranslation()
    {
        foreach (var (text, textGetter) in this.CategoryTitles)
        {
            text.SetText(textGetter());
        }
    }
}