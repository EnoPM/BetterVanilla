using System;
using BetterVanilla.Core;
using BetterVanilla.Localization;
using UnityEngine;

namespace BetterVanilla.Ui;

public sealed class UiManager : MonoBehaviour
{
    public static UiManager? Instance { get; private set; }
    
    public ModMenuButtons buttons = null!;
    public ModMenu modMenu = null!;
    
    public ColorPicker colorPickerPrefab = null!;
    public TextField textFieldPrefab = null!;
    public DropdownField dropdownFieldPrefab = null!;
    public NumberField numberFieldPrefab = null!;
    public ToggleField toggleFieldPrefab = null!;
    
    private readonly UiInteractionBlocker _blocker = new();

    private void Start()
    {
        OnCloseMenuButtonClicked();
        Instance = this;
    }
    
    public void OnOpenMenuButtonClicked()
    {
        modMenu.gameObject.SetActive(true);
        _blocker.Block();
    }
    
    public void OnCloseMenuButtonClicked()
    {
        modMenu.gameObject.SetActive(false);
        _blocker.Unblock();
    }
    
    public void OnLocalizationChanged(int index)
    {
        Ls.LogMessage($"Localization changed: {index}");
        LocalizationManager.CurrentLanguage = index switch
        {
            0 => Language.En,
            1 => Language.Fr,
            _ => throw new InvalidOperationException($"Localization index {index} is not supported")
        };
        ModOptions.User.Options.Language.Value = LocalizationManager.CurrentLanguage.ToString();
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}