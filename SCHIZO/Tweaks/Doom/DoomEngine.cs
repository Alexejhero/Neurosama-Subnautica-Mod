using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UWE;

namespace SCHIZO.Tweaks.Doom;

internal partial class DoomEngine : MonoBehaviour
{
    private static DoomEngine _instance;
    private static GameObject _instanceGO;
    public static DoomEngine Instance
    {
        get
        {
            if (_instance) return _instance;
            if (!_instanceGO)
            {
                _instanceGO = new GameObject(nameof(DoomEngine));
                DontDestroyOnLoad(_instanceGO);
                _instanceGO.EnsureComponent<SceneCleanerPreserve>(); // b r u h
            }
            _instance = _instanceGO.EnsureComponent<DoomEngine>();
            return _instance;
        }
    }
    public Texture2D Screen { get; } = new Texture2D(0, 0, TextureFormat.BGRA32, false);
    public Vector2Int ScreenResolution => new(Screen.width, Screen.height);

    /// <summary>
    /// Whether the player has been initialized at least once.
    /// </summary>
    public bool IsInitialized => _doomThread.ThreadState != System.Threading.ThreadState.Unstarted;
    /// <summary>
    /// Whether the game has started.
    /// </summary>
    public bool IsStarted { get; private set; }
    /// <summary>
    /// If this is <see langword="false"/>, the game is paused externally (e.g. if there are no <see cref="IDoomClient"/>s connected).
    /// </summary>
    public bool IsRunning => _runningEvent.IsSet;
    public string WindowTitle { get; private set; }
    private DoomClientManager _clientManager;

    internal int ConnectedClients => _clientManager.Count;
    internal float StartupTime { get; private set; }
    internal static int LastExitCode { get; private set; }
    internal int CurrentTick { get; private set; }

    private void Awake()
    {
        _clientManager = new(this);
    }

    private void OnDestroy()
    {
        _threadStop.Set();
    }
    private void Initialize()
    {
        if (IsInitialized) return;

        _doomThread.Name = "Doom";
        _doomThread.Priority = System.Threading.ThreadPriority.BelowNormal;
        _doomThread.IsBackground = true;
        if (!CurrentThreadIsMainThread())
            throw new InvalidOperationException("Doom must be initialized from the Unity thread");
        _mainThread = Thread.CurrentThread;
        _mainThreadId = _mainThread.ManagedThreadId;
        _doomThread.Start();
    }

    internal void RunOnUnityThread(Action action)
    {
        if (IsOnUnityThread())
            action();
        else
            ScheduleUnityThread(action);
    }
    private ConcurrentQueue<Action> _unityThreadQueue = new();
    internal void ScheduleUnityThread(Action action)
    {
        _unityThreadQueue.Enqueue(action);
    }

    private static bool IsOnUnityThread()
    {
        if (_mainThreadId == 0)
            LOGGER.LogWarning("Trying to check Unity thread but we don't know which one it is yet");
        return Thread.CurrentThread.ManagedThreadId == _mainThreadId;
    }

    public void Connect(IDoomClient client)
    {
        if (!IsInitialized) Initialize();
        else if (!IsOnUnityThread())
            throw new InvalidOperationException("Clients must connect from the Unity thread");
        _clientManager.Add(client);
    }

    public void Disconnect(IDoomClient client)
    {
        // theoretically this should only ever get called from the unity thread
        if (!IsInitialized)
        {
            LogError("DoomPlayer should have been initialized by the time Disconnect gets called");
            Initialize();
        }
        _clientManager.Remove(client);
    }

    public void SetPaused(bool paused)
    {
        if (paused == !IsRunning) return;
        if (!IsOnUnityThread())
        {
            LogWarning("doom thread calling SetPaused, it's about to ouroboros itself");
        }
        if (paused)
        {
            _runningEvent.Reset();
            DoomFmodAudio.PauseMusic();
        }
        else
        {
            _runningEvent.Set();
            DoomFmodAudio.ResumeMusic();
        }
    }

    private void Update()
    {
        if (!IsStarted) return;
        bool shouldBeRunning = Player.main && !FreezeTime.HasFreezers() && ConnectedClients > 0;
        if (IsRunning != shouldBeRunning)
        {
            SetPaused(!shouldBeRunning);
        }
        while (_unityThreadQueue.TryDequeue(out Action action))
        {
            action();
        }
        if (_clientManager.Any(c => c.IsAcceptingInput))
        {
            CollectKeys();
            CollectMouse();
        }
        if (_frameState == FrameState.GatherInput)
        {
            _frameState = FrameState.DoGameTick;
            _inputEvent.Set();
        }
        // game drew a frame, notify clients
        if (_frameState == FrameState.WaitForDraw)
        {
            _frameState = FrameState.FrameEnd;
            if (_screenBuffer == IntPtr.Zero)
            {
                // theoretically should never happen
                LogError("Tried to draw before screen buffer was assigned");
                return;
            }
            Screen.LoadRawTextureData(_screenBuffer, Screen.width * Screen.height * sizeof(uint));
            Screen.Apply();
            _clientManager.OnDrawFrame();
            _drawEvent.Set();
        }
    }

    // without the lock, the main thread randomly freezes (presumably due to simultaneous access to _XXXKeys)
    private object _inputSync = new();
    // holding input state until the doom thread consumes it
    private readonly HashSet<DoomKey> _pressedKeys = [];
    private readonly HashSet<DoomKey> _heldKeys = [];
    private readonly HashSet<DoomKey> _releasedKeys = [];
    // the blocklist was needed previously because we mimic bindings through sending different keys (e.g. S sends "down arrow")
    // unfortunately, it causes inputs to double up w/ the "input string"
    // e.g. pause menu "save game" shortcut 's' conflicts w/ "down arrow" binding for S
    // to avoid a complicated solution, i just moved the shortcuts in C :)
    //private readonly HashSet<byte> _blockChars = new(Encoding.ASCII.GetBytes("swe"));

    private void CollectKeys()
    {
        if (!CurrentThreadIsMainThread())
        {
            LogError("Should not be in CollectKeys on background thread");
            Debugger.Break();
        }
        lock (_inputSync)
        {
            _releasedKeys.AddRange(_heldKeys);
            if (!Input.anyKey)
            {
                _heldKeys.Clear();
                return;
            }

            foreach ((KeyCode unityKey, DoomKey doomKey) in KeyCodeConverter.GetAllKeys())
            {
                if (!Input.GetKey(unityKey)) continue;

                if (_heldKeys.Add(doomKey))
                {
                    //LogDebug($"CollectKeys pressed {doomKey} ({unityKey})");
                    _pressedKeys.Add(doomKey);
                }
                // alternate binds, duplicate DoomKey values, etc.
                // otherwise it counts as pressed and released in the same tick and therefore the input gets dropped or spammed
                _releasedKeys.Remove(doomKey);
            }
            // 6/10 on the funny scale
            // but there are a shitload of keys checked in the C code that aren't in enums at all
            // and i cba to redefine them all
            _pressedKeys.AddRange(Encoding.ASCII.GetBytes(Input.inputString.ToLowerInvariant())
                //.Where(c => !_blockChars.Contains(c))
                .Select(KeyCodeConverter.ConvertKey)
                .Where(k => k != default && _heldKeys.Add(k)));

            _releasedKeys.RemoveRange(_pressedKeys);
            _heldKeys.RemoveRange(_releasedKeys);
        }
    }

    private float _mouseDeltaX;
    private float _mouseDeltaY;
    private float _mouseWheelDelta;
    private bool _left;
    private bool _right;
    private bool _middle;
    private void CollectMouse()
    {
        lock (_inputSync)
        {
            _mouseDeltaX += Input.GetAxis("Mouse X");
            //_mouseDeltaY += Input.GetAxis("Mouse Y"); // this controls forward/back movement which feels mega weird
            _mouseWheelDelta += Input.mouseScrollDelta.y;
            if (_ignoringLeftClick)
            {
                _left = false;
                if (Input.GetMouseButtonUp(0))
                    _ignoringLeftClick = false;
            }
            else
            {
                _left = Input.GetMouseButton(0);
            }

            _right = Input.GetMouseButton(1);
            _middle = Input.GetMouseButton(2);
        }
    }

    // here to prevent bleeding the "Click to play DOOM" click through to the game
    private bool _ignoringLeftClick;
    internal void IgnoreNextLeftClick() => _ignoringLeftClick = true;
}
