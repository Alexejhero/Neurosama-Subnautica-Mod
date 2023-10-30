namespace SCHIZO.Helpers;

public static class ConsoleHelpers
{
    public static bool TryParseBoolean(string str, out bool value)
    {
        bool? parsed = str.ToLowerInvariant() switch
        {
            "true" or "1" or "yes" or "on" or "start" or "enable" => true,
            "false" or "0" or "no" or "off" or "end" or "stop" or "disable" => false,
            _ => null
        };

        value = parsed ?? default;
        return parsed != null;
    }
}
