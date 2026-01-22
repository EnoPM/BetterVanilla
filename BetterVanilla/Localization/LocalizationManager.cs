using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using BetterVanilla.Core;

namespace BetterVanilla.Localization;

public sealed class LocalizationManager : INotifyPropertyChanged
{
    private const string DefaultLanguage = "en";
    private const string ResourcePrefix = "BetterVanilla.Assets.Locales.";

    private static readonly Lazy<LocalizationManager> LazyInstance = new(() => new LocalizationManager());
    public static LocalizationManager Instance => LazyInstance.Value;

    public event Action? LanguageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Dictionary<string, LanguageData> _languages = new();
    private LanguageData? _currentLanguage;
    private LanguageData? _fallbackLanguage;

    public string CurrentLanguageCode => _currentLanguage?.Code ?? DefaultLanguage;
    public string CurrentLanguageName => _currentLanguage?.Name ?? "English";

    public string[] AvailableLanguages { get; private set; } = [];
    public string[] AvailableLanguageCodes { get; private set; } = [];

    public string this[string key] => Get(key);

    private LocalizationManager()
    {
        LoadAllLanguages();
        SetLanguage(DefaultLanguage);
    }

    public string Get(string key)
    {
        if (_currentLanguage != null && _currentLanguage.Translations.TryGetValue(key, out var translation))
        {
            return translation;
        }

        if (_fallbackLanguage != null && _fallbackLanguage.Translations.TryGetValue(key, out var fallbackTranslation))
        {
            return fallbackTranslation;
        }

        Ls.LogWarning($"[Localization] Missing translation for key: {key}");
        return $"[{key}]";
    }

    public void SetLanguage(string languageCode)
    {
        if (!_languages.TryGetValue(languageCode, out var language))
        {
            Ls.LogWarning($"[Localization] Language '{languageCode}' not found, falling back to '{DefaultLanguage}'");
            languageCode = DefaultLanguage;
            if (!_languages.TryGetValue(languageCode, out language))
            {
                Ls.LogError("[Localization] Default language not found!");
                return;
            }
        }

        if (_currentLanguage?.Code == language.Code)
        {
            return;
        }

        _currentLanguage = language;
        Ls.LogInfo($"[Localization] Language set to: {language.Name} ({language.Code})");

        OnPropertyChanged(nameof(CurrentLanguageCode));
        OnPropertyChanged(nameof(CurrentLanguageName));
        LanguageChanged?.Invoke();
    }

    public int GetCurrentLanguageIndex()
    {
        var index = Array.IndexOf(AvailableLanguageCodes, CurrentLanguageCode);
        return index >= 0 ? index : 0;
    }

    public void SetLanguageByIndex(int index)
    {
        if (index >= 0 && index < AvailableLanguageCodes.Length)
        {
            SetLanguage(AvailableLanguageCodes[index]);
        }
    }

    private void LoadAllLanguages()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        var languageNames = new List<string>();
        var languageCodes = new List<string>();

        foreach (var resourceName in resourceNames)
        {
            if (!resourceName.StartsWith(ResourcePrefix) || !resourceName.EndsWith(".json"))
            {
                continue;
            }

            var languageCode = resourceName
                .Replace(ResourcePrefix, "")
                .Replace(".json", "");

            var languageData = LoadLanguage(assembly, resourceName, languageCode);
            if (languageData != null)
            {
                _languages[languageCode] = languageData;
                languageNames.Add(languageData.Name);
                languageCodes.Add(languageCode);

                if (languageCode == DefaultLanguage)
                {
                    _fallbackLanguage = languageData;
                }

                Ls.LogInfo($"[Localization] Loaded language: {languageData.Name} ({languageCode})");
            }
        }

        AvailableLanguages = languageNames.ToArray();
        AvailableLanguageCodes = languageCodes.ToArray();
    }

    private static LanguageData? LoadLanguage(Assembly assembly, string resourceName, string languageCode)
    {
        try
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Ls.LogWarning($"[Localization] Could not load resource: {resourceName}");
                return null;
            }

            var buffer = new byte[stream.Length];
            _ = stream.Read(buffer, 0, buffer.Length);
            var json = Encoding.UTF8.GetString(buffer);

            var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var metadata = root.GetProperty("metadata");
            var languageName = metadata.GetProperty("language").GetString() ?? languageCode;

            var translations = new Dictionary<string, string>();
            var translationsElement = root.GetProperty("translations");

            foreach (var property in translationsElement.EnumerateObject())
            {
                translations[property.Name] = property.Value.GetString() ?? string.Empty;
            }

            return new LanguageData(languageCode, languageName, translations);
        }
        catch (Exception ex)
        {
            Ls.LogError($"[Localization] Failed to load language '{languageCode}': {ex.Message}");
            return null;
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class LanguageData(string code, string name, Dictionary<string, string> translations)
    {
        public string Code { get; } = code;
        public string Name { get; } = name;
        public Dictionary<string, string> Translations { get; } = translations;
    }
}
