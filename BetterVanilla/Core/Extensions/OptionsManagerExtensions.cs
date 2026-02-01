using System;
using BetterVanilla.Options.Core;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui;
using BetterVanilla.Ui.Base;
using Object = UnityEngine.Object;

namespace BetterVanilla.Core.Extensions;

public static class OptionsManagerExtensions
{
    extension<TOptionsHolder>(OptionsManager<TOptionsHolder> optionsManager) where TOptionsHolder : OptionsHolderBase, new()
    {
        public ColorPicker AddToTab(TabBase tab, ColorOption option)
        {
            if (UiManager.Instance == null)
            {
                throw new InvalidOperationException($"{nameof(UiManager)} is not initialized.");
            }
            var component = Object.Instantiate(UiManager.Instance.colorPickerPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;
            
            return component;
        }
    }
}