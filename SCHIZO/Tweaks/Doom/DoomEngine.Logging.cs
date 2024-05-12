using BepInEx.Logging;

namespace SCHIZO.Tweaks.Doom;
partial class DoomEngine
{
    internal static ManualLogSource LogSource { get; private set; } = Logger.CreateLogSource("DOOM");

    internal static void Log(LogLevel level, string message) => LogSource.Log(level, message);
    internal static void LogDebug(string message) => LogSource.LogDebug(message);
    internal static void LogInfo(string message) => LogSource.LogInfo(message);
    internal static void LogMessage(string message) => LogSource.LogMessage(message);
    internal static void LogWarning(string message) => LogSource.LogWarning(message);
    internal static void LogError(string message) => LogSource.LogError(message);
    internal static void LogFatal(string message) => LogSource.LogFatal(message);
}
