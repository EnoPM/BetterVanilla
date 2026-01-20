using EnoUnityLoader.Logging;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Static logger for the UI library.
/// Must be initialized with SetLogSource before use.
/// </summary>
public static class UiLogger
{
    private static ManualLogSource? _logger;

    /// <summary>
    /// Sets the log source for the UI library.
    /// </summary>
    public static void SetLogSource(ManualLogSource logSource)
    {
        _logger = logSource;
    }

    public static void LogError(object data) => _logger?.LogError(data);
    public static void LogWarning(object data) => _logger?.LogWarning(data);
    public static void LogMessage(object data) => _logger?.LogMessage(data);
    public static void LogInfo(object data) => _logger?.LogInfo(data);
    public static void LogDebug(object data) => _logger?.LogDebug(data);
}
