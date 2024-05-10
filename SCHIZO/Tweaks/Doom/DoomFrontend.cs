using System;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal class DoomFrontend : MonoBehaviour, IDoomClient
{
    private DoomEngine engine => DoomEngine.Instance;
    public Texture2D DoomScreen => engine.Screen;
    public Action Connected;
    public Action Disconnected;
    public Action<int> Exited;
    public bool PlayerPlaying { get; set; }
    public bool IsAcceptingInput => PlayerPlaying;
    public string WindowTitle { get; private set; }
    private void OnDestroy()
    {
        Disconnect();
    }
    public void Connect() => engine.Connect(this);
    public void Disconnect() => engine.Disconnect(this);
    public void OnConnected()
    {
        Connected?.Invoke();
    }
    public void OnDisconnected()
    {
        Disconnected?.Invoke();
    }
    public void OnDoomInit()
    {
    }

    public void OnDoomFrame() { }
    public void OnDoomTick() { }
    public void OnDoomExit(int exitCode)
    {
        enabled = false;
        Exited?.Invoke(exitCode);
    }

    public void OnWindowTitleChanged(string title)
    {
        WindowTitle = title;
    }
}
