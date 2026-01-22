namespace BetterVanilla.Localization;

/// <summary>
/// Constants for translation keys.
/// Usage: LocalizationManager.Instance[TranslationKey.Menu.Title]
/// </summary>
public static class TranslationKey
{
    public static class Menu
    {
        public const string Title = "menu.title";
        public const string Close = "menu.close";
        public const string BecomeSponsorButton = "menu.becomeSponsorButton";
    }

    public static class Options
    {
        public const string DisplayVentsInMap = "options.displayVentsInMap";
        public const string Language = "options.language";
    }

    public static class Common
    {
        public const string Yes = "common.yes";
        public const string No = "common.no";
        public const string Cancel = "common.cancel";
        public const string Confirm = "common.confirm";
        public const string Save = "common.save";
        public const string Reset = "common.reset";
    }
}
