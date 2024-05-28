using BepInEx.Logging;

namespace SCHIZO.Commands.Output;
public sealed class LogSink : ISink
{
    public required LogLevel LogLevel { get; init; }

    public static readonly LogSink Debug = new() { LogLevel = LogLevel.Debug };
    public static readonly LogSink Info = new() { LogLevel = LogLevel.Info };
    public static readonly LogSink Message = new() { LogLevel = LogLevel.Message };
    public static readonly LogSink Warning = new() { LogLevel = LogLevel.Warning };
    public static readonly LogSink Error = new() { LogLevel = LogLevel.Error };
    public static readonly LogSink Fatal = new() { LogLevel = LogLevel.Fatal };
    private LogSink() { }

    public bool TryConsume(ref object output)
    {
        if (output != null)
            LOGGER.Log(LogLevel, output);
        return false;
    }
}
