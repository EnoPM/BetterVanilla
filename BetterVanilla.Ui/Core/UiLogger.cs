using System.Reflection;
using EnoUnityLoader.Logging;

namespace BetterVanilla.Ui.Core;

/// <summary>
/// Static logger for the UI library.
/// Must be initialized with SetLogSource before use.
/// </summary>
public static class UiLogger
{
    private static readonly ManualLogSource Logger = EnoUnityLoader.Logging.Logger.CreateLogSource(Assembly.GetExecutingAssembly().GetName().Name ?? nameof(UiLogger));

    public static void LogError(object data) => Logger.LogError(data);
    public static void LogWarning(object data) => Logger.LogWarning(data);
    public static void LogMessage(object data) => Logger.LogMessage(data);
    public static void LogInfo(object data) => Logger.LogInfo(data);
    public static void LogDebug(object data) => Logger.LogDebug(data);
}
