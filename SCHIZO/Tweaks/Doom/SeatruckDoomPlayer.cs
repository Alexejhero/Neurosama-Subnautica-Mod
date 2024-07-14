using System.Collections.Generic;
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
    private PowerRelay _power;
    private Vector3 _oldScreenScale;
    private bool _die;
    public float hibernateTime = 30f;
    private float _hibernateTimer;

    private void OnEnable()
    {
        if (DoomEngine.LastExitCode != 0 || !DoomNative.CheckDll())
        {
            Destroy(this);
            return;
        }
        // seatruck modules have no discriminator, they're just prefabs
        _pictureFrame = transform.parent.Find("PictureFrame");
        if (!_pictureFrame)
        {
            Destroy(this);
            return;
        }
        _ghost = MainCameraControl.main.gameObject.GetComponent<FreecamController>();
        _power = transform.parent.GetComponent<PowerRelay>();
        _pictureFrame.GetComponent<PictureFrame>().enabled = false;
        // here's where we would move the frame to be eye-level
        // but it's part of the seatruck mesh...

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
        _connection.Connected += PlayerConnected;
        _connection.Disconnected += PlayerDisconnected;
        _connection.Exited += OnDoomExit;

        _ourHandTarget = _handTrigger.gameObject.AddComponent<GenericHandTarget>();
        _ourHandTarget.onHandHover = new();
        _ourHandTarget.onHandHover.AddListener(OnHandHover);
        _ourHandTarget.onHandClick = new();
        _ourHandTarget.onHandClick.AddListener(OnHandClick);
    }

    private void OnDoomExit(int code)
    {
        if (code != 0) _die = true; // thread
    }

    private void OnDisable()
    {
        IsControlling = false;
        if (_connection)
        {
            _connection.enabled = false;
            _connection.Connected -= PlayerConnected;
            _connection.Disconnected -= PlayerDisconnected;
            _connection.Exited -= OnDoomExit;
            Destroy(_connection);
        }
        if (_ourHandTarget)
        {
            _oldHandTarget.enabled = true;
            Destroy(_ourHandTarget);
        }
        if (!_pictureFrame) return;

        _screen.localScale = _oldScreenScale;

        _pictureFrame.GetComponent<PictureFrame>().enabled = true;
    }

    private void OnHandHover(HandTargetEventData eventData)
    {
        if (IsControlling) return;
        if (!_power.IsPowered())
        {
            HandReticle.main.SetText(HandReticle.TextType.Hand, "Unpowered", false);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "", false);
            HandReticle.main.SetIcon(HandReticle.IconType.HandDeny);
            return;
        }

        HandReticle.main.SetText(HandReticle.TextType.Hand, $"Play {DoomEngine.Instance.WindowTitle ?? "DOOM"}", false, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "", false);
        HandReticle.main.SetIcon(HandReticle.IconType.Interact);
    }
    private void OnHandClick(HandTargetEventData eventData)
    {
        if (!_power.IsPowered()) return;
        if (IsControlling) return;

        _connection.enabled = true;
        IsControlling = true;
    }
    private bool _hintUnderstood;
    private bool _controlling;
    public bool IsControlling
    {
        get => _controlling;
        set
        {
            if (_controlling == value) return;

            _controlling = value;
            DoomEngine.Instance.IgnoreNextLeftClick();
            ToggleGameInput(!value);
            _connection.PlayerPlaying = value;
            _handTrigger.gameObject.SetActive(!value);
            FreecamToScreen(value);
            ToggleCrosshair(value);
            if (value)
            {
                // todo make another transform below (so it comes from a "speaker")
                DoomFmodAudio.Emitter = _screen; // center of screen
                _connection.Connect();
                if (!_hintUnderstood) ShowExitHint();
            }
            else
            {
                _hintUnderstood = true;
                Hint.main.message.Hide();
            }
        }
    }

    private void ToggleCrosshair(bool value)
    {
        if (value)
            HandReticle.main.RequestCrosshairHide();
        else
            HandReticle.main.UnrequestCrosshairHide();
    }

    private static void ShowExitHint()
    {
        uGUI_PopupMessage msg = Hint.main.message;
        msg.SetText("Press <color=yellow>Ctrl+Q</color> to exit", TextAnchor.MiddleLeft);
        msg.Show(-1, 0f, 0.25f, 0.25f, null);
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

        GameInput.moveDirection = Vector3.zero;
        GameInput.ClearInput();
        GameInput.instance.enabled = enable;
    }

    private static float _screenDistance = 0.5f;
    private FreecamController _ghost;
    private void FreecamToScreen(bool looking)
    {
        if (looking == _ghost.mode) return;

        _ghost.FreecamToggle();
        //ghost.ghostMode = looking;

        if (!looking)
        {
            Transform cam = SNCameraRoot.main.transform;
            cam.localPosition = Vector3.zero;
            cam.localRotation = Quaternion.identity;
            return;
        }
        Vector3 normal = _screenRenderer.transform.forward;
        Vector3 center = _screenRenderer.bounds.center;
        _ghost.tr.position = center - normal * _screenDistance;
        _ghost.tr.LookAt(center);
    }

    private readonly Queue<float> _escapePresses = [];
    private float _escapeHeld;
    private void Update()
    {
        if (_die)
        {
            Destroy(this);
            return;
        }

        if (!_power.IsPowered())
        {
            ShutDownScreen();
        }
        if (IsControlling)
        {
            _hibernateTimer = 0;
            if (Input.GetKey(KeyCode.Escape))
            {
                _escapeHeld += Time.deltaTime;
                if (_escapeHeld > 1f)
                    ShowExitHint();
            }
            else
            {
                _escapeHeld = 0f;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 3 presses in 5 seconds
                while (_escapePresses.Count > 0 && Time.time - _escapePresses.Peek() > 5f)
                    _escapePresses.Dequeue();

                _escapePresses.Enqueue(Time.time);
                if (_escapePresses.Count >= 3)
                    ShowExitHint();
            }
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl))
            {
                IsControlling = false;
                _escapeHeld = 0;
                _escapePresses.Clear();
            }
        }
        else if (_connection.enabled)
        {
            Vector3 toPlayer = Player.main.transform.position - transform.position;
            if (toPlayer.sqrMagnitude > 900f || _hibernateTimer > hibernateTime)
                ShutDownScreen();
            _hibernateTimer += Time.deltaTime;
        }
    }

    private void ShutDownScreen()
    {
        IsControlling = false;
        if (_connection.enabled)
        {
            _connection.Disconnect();
            _connection.enabled = false;
        }
    }
}
