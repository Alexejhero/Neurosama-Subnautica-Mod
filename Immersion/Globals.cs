using Immersion.Formatting;

namespace Immersion;

public static class Globals
{
    private const string PlayerNameKey = $"{nameof(Immersion)}_PlayerName";
    private const string BaseUrlKey = $"{nameof(Immersion)}_BaseURL";
    private const string PronounsKey = $"{nameof(Immersion)}_Pronouns";

    private const string DefaultPlayerName = "Vedal";
    private const string DefaultUrl = "http://localhost:8000/subnautica/";
    private static readonly PronounSet DefaultPronouns = PronounSet.HeHim;

    public static string PlayerName
    {
        get => PlayerPrefs.GetString(PlayerNameKey, DefaultPlayerName);
        set => PlayerPrefs.SetString(PlayerNameKey, value);
    }
    public static string BaseUrl
    {
        get => PlayerPrefs.GetString(BaseUrlKey, DefaultUrl);
        set => PlayerPrefs.SetString(BaseUrlKey, value);
    }

    private static PronounSet? _playerPronouns;
    public static PronounSet PlayerPronouns
    {
        get
        {
            if (_playerPronouns.HasValue)
                return _playerPronouns.Value;

            if (!PronounSet.TryParse(PlayerPrefs.GetString(PronounsKey, ""), out PronounSet pronouns))
                _playerPronouns = DefaultPronouns;
            PlayerPronouns = pronouns;
            return _playerPronouns.Value;
        }
        set
        {
            _playerPronouns = value;
            PlayerPrefs.SetString(PronounsKey, value.ToString());
        }
    }
}
