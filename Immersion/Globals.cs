using Immersion.Formatting;

namespace Immersion;

public static class Globals
{
    private const string _PLAYER_NAME_PLAYERPREFS_KEY = "Immersion_PlayerName";
    private const string _BASEURL_PLAYARPREFS_KEY = "Immersion_BaseURL";
    private const string _PRONOUNS_PLAYARPREFS_KEY = "Immersion_BaseURL";

    public static string PlayerName
    {
        get => PlayerPrefs.GetString(_PLAYER_NAME_PLAYERPREFS_KEY, "Vedal");
        set => PlayerPrefs.SetString(_PLAYER_NAME_PLAYERPREFS_KEY, value);
    }
    public static PronounSet PlayerPronouns { get; set; } = PronounSet.HeHim;

    public static string BaseUrl
    {
        get => PlayerPrefs.GetString(_BASEURL_PLAYARPREFS_KEY, "http://localhost:8000/subnautica/");
        set => PlayerPrefs.SetString(_BASEURL_PLAYARPREFS_KEY, value);
    }
}
