using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

partial class SeatruckDoomPlayer : MonoBehaviour
{
    private Transform _pictureFrame;
    private Transform _screen;
    private Transform _handTrigger;
    private MeshRenderer _screenRenderer;
    private GenericHandTarget _oldHandTarget;
    private GenericHandTarget _ourHandTarget;
    private DoomFrontend _connection;
    private Vector3 _oldScreenScale;
    private void OnEnable()
    {
        // seatruck modules have no discriminator, they're just prefabs
        _pictureFrame = transform.parent.Find("PictureFrame");
        if (!_pictureFrame)
        {
            Destroy(this);
            return;
        }
        _pictureFrame.GetComponent<PictureFrame>().enabled = false;

        _handTrigger = _pictureFrame.Find("Trigger");
        _oldHandTarget = _handTrigger.GetComponent<GenericHandTarget>();
        _oldHandTarget.enabled = false;

        _screen = _pictureFrame.Find("Screen");
        _screenRenderer = _screen.GetComponent<MeshRenderer>();
        _screenRenderer.enabled = true;
        // the screen is cut off a little bit vertically
        _oldScreenScale = _screen.localScale;
        _screen.localScale = new Vector3(1.3f, 0.75f, 1);

        _connection = _screen.gameObject.AddComponent<DoomFrontend>();
        _connection.enabled = false;
        _connection.Connected += PlayerConnected;
        _connection.Disconnected += PlayerDisconnected;
        _connection.enabled = true;

        _ourHandTarget = _handTrigger.gameObject.AddComponent<GenericHandTarget>();
        _ourHandTarget.onHandHover = new();
        _ourHandTarget.onHandHover.AddListener(OnHandHover);
        _ourHandTarget.onHandClick = new();
        _ourHandTarget.onHandClick.AddListener(OnHandClick);
    }

    private void OnDisable()
    {
        if (!_pictureFrame) return;
        _connection.enabled = false;
        _connection.Connected -= PlayerConnected;
        _connection.Disconnected -= PlayerDisconnected;
        Destroy(_connection);

        _oldHandTarget.enabled = true;
        Destroy(_ourHandTarget);

        _screen.localScale = _oldScreenScale;

        _pictureFrame.GetComponent<PictureFrame>().enabled = true;
    }

    private void OnHandHover(HandTargetEventData eventData)
    {
        if (IsControlling) return;

        HandReticle.main.SetText(HandReticle.TextType.Hand, $"Play {DoomEngine.Instance.WindowTitle ?? "DOOM"}", false, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "", false);
        HandReticle.main.SetIcon(HandReticle.IconType.Interact);
    }

    private bool _controlling;
    public bool IsControlling
    {
        get => _controlling;
        set
        {
            if (_controlling == value) return;

            _controlling = value;
            ToggleGameInput(!value);
            _connection.PlayerPlaying = value;
            _handTrigger.gameObject.SetActive(!value);
        }
    }
    private void OnHandClick(HandTargetEventData eventData)
    {
        _connection.enabled = true;
        IsControlling = true;
    }
    private Texture _oldTex;
    private void PlayerConnected()
    {
        if (_screenRenderer)
        {
            _oldTex = _screenRenderer.sharedMaterial.mainTexture;
            _screenRenderer.sharedMaterial.mainTexture = _connection.DoomScreen;
            Vector2 scale = _screenRenderer.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            _screenRenderer.sharedMaterial.mainTextureScale = scale;
        }
        else
        {
            LOGGER.LogWarning("(Seatruck DOOM Player) Screen should have a mesh renderer");
        }
    }
    private void PlayerDisconnected()
    {
        if (_screenRenderer)
        {
            _screenRenderer.sharedMaterial.mainTexture = _oldTex;
            Vector2 scale = _screenRenderer.sharedMaterial.mainTextureScale;
            scale.y *= -1;
            _screenRenderer.sharedMaterial.mainTextureScale = scale;
        }
        IsControlling = false; // just in case
    }
    private void ToggleGameInput(bool enable)
    {
        if (GameInput.instance.enabled == enable) return;

        GameInput.ClearInput();
        GameInput.instance.enabled = enable;
        //Player.main.playerController.SetEnabled(!locked);
        //FPSInputModule.current.lockMovement = locked;
        //FPSInputModule.current.lockRotation = locked;
        //FPSInputModule.current.lockPauseMenu = locked;
    }

    private void Update()
    {
        // todo show hint
        /// <see cref="Hint.main"/> and <see cref="uGUI_PopupMessage"/>
        if (IsControlling && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl))
        {
            IsControlling = false;
        }
    }
}
