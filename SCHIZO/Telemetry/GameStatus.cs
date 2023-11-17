using System;
using Nautilus.Utility;

namespace SCHIZO.Telemetry;

partial class GameStatus
{
    public static event Action Loading;
    public static event Action Loaded;
    public static event Action Saved;
    public static event Action QuitToMenu;

    private void Start()
    {
        OnEvent(startup);
        SaveUtils.RegisterOnStartLoadingEvent(() => OnEvent(null, Loading));
        SaveUtils.RegisterOnFinishLoadingEvent(() => OnEvent(loadedGame, Loaded));
        SaveUtils.RegisterOnSaveEvent(() => OnEvent(savedGame, Saved));
        SaveUtils.RegisterOnQuitEvent(() => OnEvent(quitToMenu, QuitToMenu));
    }

    private void OnApplicationQuit()
    {
        OnEvent(shutdown);
    }

    private void OnEvent(string endpoint, Action callback = null)
    {
        if (!string.IsNullOrEmpty(endpoint))
            SendTelemetry(endpoint);
        callback?.Invoke();
    }
}
