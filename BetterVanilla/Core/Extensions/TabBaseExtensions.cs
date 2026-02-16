using System;
using System.Collections.Generic;
using BetterVanilla.Options.Core.OptionTypes;
using BetterVanilla.Ui;
using BetterVanilla.Ui.Base;
using TMPro;

namespace BetterVanilla.Core.Extensions;

public static class TabBaseExtensions
{
    private static UiManager Manager => UiManager.Instance ?? throw new InvalidOperationException($"{nameof(UiManager)} is not initialized.");
    private static readonly Dictionary<Type, List<(TextMeshProUGUI Text, Func<string> TextGetter)>> AllCategoryTitles = [];

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

        public TextField AddOption(BetterVanilla.Options.Core.OptionTypes.StringOption option)
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

        public TextMeshProUGUI AddCategoryTitle(Func<string> textGetter)
        {
            var text = UnityEngine.Object.Instantiate(Manager.categoryTitlePrefab, tab.container);
            text.SetText(textGetter());
            
            tab.CategoryTitles.Add((text, textGetter));

            return text;
        }

        public List<(TextMeshProUGUI Text, Func<string> TextGetter)> CategoryTitles
        {
            get
            {
                if (!AllCategoryTitles.TryGetValue(tab.GetType(), out var list))
                {
                    list = AllCategoryTitles[tab.GetType()] = [];
                }

                return list;
            }
        }
        
        public bool IsHeaderActive => tab.header != null && tab.header.gameObject.active && tab.header.title != null;
    }
}