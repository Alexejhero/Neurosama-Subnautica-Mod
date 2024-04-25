using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Tweaks.Doom;

internal class DoomPlayerConnection : MonoBehaviour, IDoomClient
{
    private DoomEngine player => DoomEngine.Instance;
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
    private Texture _oldTex;
    public void OnDoomInit()
    {
        // temporary 
        Image image = GetComponent<Image>();
        if (image)
        {
            _oldSprite = image.sprite;
            image.sprite = DoomScreenSprite;
            // screen buffer is flipped vertically
            Vector3 scale = image.rectTransform.localScale;
            scale.y *= -1;
            image.rectTransform.localScale = scale;
            return;
        }
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh)
        {
            _oldTex = mesh.sharedMaterial.mainTexture;
            mesh.sharedMaterial.mainTexture = DoomScreen;
            Vector2 scale = mesh.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            mesh.sharedMaterial.mainTextureScale = scale;
        }
    }
    public void OnDisconnected() { }
    public void OnDoomFrame() { }
    public void OnDoomTick() { }
    public void OnDoomExit(int exitCode)
    {
        Image image = GetComponent<Image>();
        if (image)
        {
            image.sprite = _oldSprite;
            // unflip
            Vector3 scale = image.rectTransform.localScale;
            scale.y *= -1;
            image.rectTransform.localScale = scale;
            return;
        }
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh)
        {
            mesh.sharedMaterial.mainTexture = _oldTex;
            Vector2 scale = mesh.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            mesh.sharedMaterial.mainTextureScale = scale;
        }
    }

    public void OnWindowTitleChanged(string title)
    {
    }
}
