using System;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal class DoomPlayerConnection : MonoBehaviour, IDoomClient
{
    private DoomEngine player => DoomEngine.Instance;
    public Texture2D DoomScreen => player.Screen;
    public Sprite DoomScreenSprite => player.Sprite;
    public Action Connected;
    public Action Disconnected;
    private Texture _oldTex;
    public string WindowTitle { get; private set; }
    // TODO UI/UX (prompt to start, keybind to quit, place camera at screen while playing)
    private void OnEnable()
    {
        player.Connect(this);
    }
    private void OnDisable()
    {
        player.Disconnect(this);
    }
    public void OnConnected()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh)
        {
            _oldTex = mesh.sharedMaterial.mainTexture;
            mesh.sharedMaterial.mainTexture = DoomScreen;
            Vector2 scale = mesh.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            mesh.sharedMaterial.mainTextureScale = scale;
        }
        else
        {
            LOGGER.LogWarning("doom player target should have a mesh renderer");
        }
        ToggleGameInput(true);
        Connected?.Invoke();
    }
    public void OnDisconnected()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh)
        {
            mesh.sharedMaterial.mainTexture = _oldTex;
            Vector2 scale = mesh.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            mesh.sharedMaterial.mainTextureScale = scale;
        }
        ToggleGameInput(false);
        Disconnected?.Invoke();
    }
    private void ToggleGameInput(bool locked)
    {
        GameInput.instance.enabled = !locked;
        //Player.main.playerController.SetEnabled(!locked);
        //FPSInputModule.current.lockMovement = locked;
        //FPSInputModule.current.lockRotation = locked;
        //FPSInputModule.current.lockPauseMenu = locked;
    }
    public void OnDoomInit()
    {
    }

    public void OnDoomFrame() { }
    public void OnDoomTick() { }
    public void OnDoomExit(int exitCode)
    {
        enabled = false;
    }

    public void OnWindowTitleChanged(string title)
    {
        WindowTitle = title;
    }
}
