using Immersion.Formatting;

namespace Immersion;

public static class Globals
{
    public static Dictionary<string, string> Strings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["player"] = "Vernal", // player name
        ["url"] = "http://localhost/", // base URL for the API (e.g. http://localhost/api/)
    };
    public static string PlayerName
    {
        get => Strings["player"];
        set => Strings["player"] = value;
    }
    public static PronounSet PlayerPronouns { get; set; } = PronounSet.HeHim;

    public static string BaseUrl
    {
        get => Strings["url"];
        set => Strings["url"] = value;
    }
}
