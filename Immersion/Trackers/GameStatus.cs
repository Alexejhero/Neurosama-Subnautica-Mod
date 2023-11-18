using Nautilus.Utility;

namespace Immersion.Trackers;

public sealed class GameStatus : Tracker
{
    #region Endpoints
    private const string STARTUP = "hello";
    private const string SHUTDOWN = "goodbye";
    private const string LOADED = "loaded";
    private const string SAVED = "saved";
    private const string QUIT = "quitToMenu";
    #endregion Endpoints

    public static event Action Loading;
    public static event Action Loaded;
    public static event Action Saved;
    public static event Action QuitToMenu;

    private void Start()
    {
        OnEvent(STARTUP);
        SaveUtils.RegisterOnStartLoadingEvent(() => OnEvent(null, Loading));
        SaveUtils.RegisterOnFinishLoadingEvent(() => OnEvent(LOADED, Loaded));
        SaveUtils.RegisterOnSaveEvent(() => OnEvent(SAVED, Saved));
        SaveUtils.RegisterOnQuitEvent(() => OnEvent(QUIT, QuitToMenu));
    }

    private void OnApplicationQuit()
    {
        OnEvent(SHUTDOWN);
    }

    private void OnEvent(string endpoint, Action callback = null)
    {
        if (!string.IsNullOrEmpty(endpoint))
            Send(endpoint);
        callback?.Invoke();
    }
}
