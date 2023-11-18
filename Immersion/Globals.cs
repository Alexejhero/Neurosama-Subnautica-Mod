namespace Immersion;

public static class Globals
{
    public static Dictionary<string, string> Strings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["player"] = "", // player name
        ["url"] = "", // base URL for the API (e.g. http://localhost/api/)
    };
    public static string PlayerName
    {
        get => Strings["player"];
        set => Strings["player"] = value;
    }
    public static string BaseUrl
    {
        get => Strings["url"];
        set => Strings["url"] = value;
    }
}
