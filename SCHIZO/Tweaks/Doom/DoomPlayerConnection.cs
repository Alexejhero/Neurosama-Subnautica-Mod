using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Tweaks.Doom;

internal class DoomPlayerConnection : MonoBehaviour, IDoomClient
{
    private DoomPlayer player => DoomPlayer.Instance;
    public Texture2D DoomScreen => player.Screen;
    public Sprite DoomScreenSprite => player.Sprite;
    private void OnEnable()
    {
        player.Connect(this);
    }
    private void OnDisable()
    {
        player.Disconnect(this);
    }
    public void OnConnected() { }
    private Sprite _oldSprite;
    public void OnDoomInit()
    {
        Image image = GetComponent<Image>();
        if (!image) return;
        image.sprite = DoomScreenSprite;
        // screen buffer is flipped vertically
        image.rectTransform.localScale = new Vector3(1, -1, 1);
    }
    public void OnDisconnected() { }
    public void OnDoomFrame() { }
    public void OnDoomTick() { }
    public void OnDoomExit(int exitCode)
    {
        Image image = GetComponent<Image>();
        if (!image) return;
        image.sprite = _oldSprite;
        image.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void OnWindowTitleChanged(string title)
    {
    }
}
