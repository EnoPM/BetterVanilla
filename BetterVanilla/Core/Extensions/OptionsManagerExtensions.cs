using System;
using BetterVanilla.Options.Core;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui;
using BetterVanilla.Ui.Base;
using Object = UnityEngine.Object;

namespace BetterVanilla.Core.Extensions;

public static class OptionsManagerExtensions
{
    private static UiManager Manager => UiManager.Instance ?? throw new InvalidOperationException($"{nameof(UiManager)} is not initialized.");

    extension<TOptionsHolder>(OptionsManager<TOptionsHolder> optionsManager) where TOptionsHolder : OptionsHolderBase, new()
    {
        public ColorPicker AddToTab(TabBase tab, ColorOption option)
        {
            var component = Object.Instantiate(Manager.colorPickerPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;

            return component;
        }

        public NumberField AddToTab(TabBase tab, FloatOption option)
        {
            var component = Object.Instantiate(Manager.numberFieldPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;

            return component;
        }

        public TextField AddToTab(TabBase tab, Options.Core.OptionTypes.StringOption option)
        {
            var component = Object.Instantiate(Manager.textFieldPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;

            return component;
        }

        public ToggleField AddToTab(TabBase tab, BoolOption option)
        {
            var component = Object.Instantiate(Manager.toggleFieldPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;

            return component;
        }

        public DropdownField AddToTab(TabBase tab, EnumOption option)
        {
            var component = Object.Instantiate(Manager.dropdownFieldPrefab, tab.container);
            component.Option = option;
            component.ValueUpdated += optionsManager.Save;

            return component;
        }
    }
}