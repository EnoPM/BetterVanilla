using BetterVanilla.Localization;
using UnityEngine;

namespace BetterVanilla.Ui.Base;

public abstract class LocalizationBehaviourBase : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SetupTranslation();
        LocalizationManager.LanguageChanged += SetupTranslation;
    }

    protected virtual void OnDisable()
    {
        LocalizationManager.LanguageChanged -= SetupTranslation;
    }
    
    protected abstract void SetupTranslation();
}