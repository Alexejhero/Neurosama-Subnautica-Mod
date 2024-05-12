namespace SCHIZO.Tweaks.Doom;

internal interface IDoomClient
{
    bool IsAcceptingInput { get; }
    void OnConnected();
    void OnDoomInit();
    void OnDoomFrame();
    void OnDoomTick();
    void OnDisconnected();
    void OnDoomExit(int exitCode);
    void OnWindowTitleChanged(string title);
}
