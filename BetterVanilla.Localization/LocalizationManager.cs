using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterVanilla.Localization;

public static class LocalizationManager
{
    public static event Action? LanguageChanged;
    private static readonly Dictionary<Language, string> _definitions;

    public static readonly Language[] AvailableLanguages;
    public static readonly string[] AvailableLanguageNames;

    public static Language CurrentLanguage
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            LanguageChanged?.Invoke();
        }
    }

    public static int CurrentLanguageIndex
    {
        get => AvailableLanguages.IndexOf(CurrentLanguage);
        set
        {
            if (CurrentLanguageIndex == value) return;
            CurrentLanguage = AvailableLanguages[value];
        }
    }

    static LocalizationManager()
    {
        _definitions = new Dictionary<Language, string>()
        {
            {
                Language.En, "English"
            },
            {
                Language.Fr, "Français"
            }
        };

        AvailableLanguages = _definitions.Keys.ToArray();
        AvailableLanguageNames = _definitions.Values.ToArray();
    }

    public static Language GetByIndex(int index) => AvailableLanguages[index];
    public static string GetNameByIndex(int index) => AvailableLanguageNames[index];
}