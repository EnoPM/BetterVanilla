using System;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui;
using BetterVanilla.Ui.Base;

namespace BetterVanilla.Core.Extensions;

public static class TabBaseExtensions
{
    private static UiManager Manager => UiManager.Instance ?? throw new InvalidOperationException($"{nameof(UiManager)} is not initialized.");

    extension(TabBase tab)
    {
        public ColorPicker AddOption(ColorOption option)
        {
            var component = UnityEngine.Object.Instantiate(Manager.colorPickerPrefab, tab.container);
            component.Option = option;

            return component;
        }
        
        public NumberField AddOption(FloatOption option)
        {
            var component = UnityEngine.Object.Instantiate(Manager.numberFieldPrefab, tab.container);
            component.Option = option;

            return component;
        }

        public TextField AddOption(Options.Core.OptionTypes.StringOption option)
        {
            var component = UnityEngine.Object.Instantiate(Manager.textFieldPrefab, tab.container);
            component.Option = option;

            return component;
        }

        public ToggleField AddOption(BoolOption option)
        {
            var component = UnityEngine.Object.Instantiate(Manager.toggleFieldPrefab, tab.container);
            component.Option = option;

            return component;
        }

        public DropdownField AddOption(EnumOption option)
        {
            var component = UnityEngine.Object.Instantiate(Manager.dropdownFieldPrefab, tab.container);
            component.Option = option;

            return component;
        }
        
        public bool IsHeaderActive => tab.header != null && tab.header.gameObject.active && tab.header.title != null;
    }
}