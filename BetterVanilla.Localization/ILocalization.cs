using System;

namespace BetterVanilla.Localization;

/// <summary>
/// Interface for generated localization classes.
/// </summary>
public interface ILocalization
{
    /// <summary>
    /// Gets the translation for the specified key.
    /// </summary>
    string Get(string key);

    /// <summary>
    /// Gets all available translation keys.
    /// </summary>
    string[] Keys { get; }

    /// <summary>
    /// Gets all supported languages.
    /// </summary>
    string[] SupportedLanguages { get; }

    /// <summary>
    /// Gets or sets the current language code.
    /// </summary>
    string CurrentLanguage { get; set; }

    /// <summary>
    /// Event raised when the current language changes.
    /// </summary>
    event Action? LanguageChanged;
}
