using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using BepInEx.Logging;
using UnityEngine;
using UWE;

namespace SCHIZO.Tweaks.Doom;

internal partial class DoomPlayer : MonoBehaviour
{
    private static DoomPlayer _instance;
    public static DoomPlayer Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = new GameObject(nameof(DoomPlayer)).AddComponent<DoomPlayer>();
            DontDestroyOnLoad(_instance.gameObject);
            return _instance;
        }
    }
    public Texture2D Screen { get; } = new Texture2D(0, 0, TextureFormat.BGRA32, false);
    public Sprite Sprite { get; private set; }
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
    internal int LastExitCode { get; private set; }
    internal int CurrentTick { get; private set; }

    private ManualLogSource LogSource { get; set; }

    private void Awake()
    {
        _clientManager = new(this);
    }

    private void OnDestroy()
    {
        _doomThread.Abort();
    }
    private void Initialize()
    {
        if (IsInitialized) return;

        LogSource = LOGGER;
        _doomThread.Name = "Doom";
        _doomThread.Priority = System.Threading.ThreadPriority.BelowNormal;
        if (!CurrentThreadIsMainThread())
            throw new InvalidOperationException("Doom must be initialized from the Unity thread");
        _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        _doomThread.Start();
    }

    internal void RunOnUnityThread(Action action)
    {
        if (IsOnUnityThread())
            action();
        else
            ScheduleUnityThread(action);
    }
    internal void ScheduleUnityThread(Action action)
    {
        StartCoroutine(Coro());
        IEnumerator Coro()
        {
            yield return null; // will resume on unity thread
            action();
        }
    }

    private static bool IsOnUnityThread()
    {
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
            LogSource.LogError("DoomPlayer should have been initialized by the time Disconnect gets called");
            Initialize();
        }
        _clientManager.Remove(client);
    }

    public void SetPaused(bool paused)
    {
        if (!IsOnUnityThread())
        {
            LogSource.LogWarning("doom thread calling SetPaused, it's about to ouroboros itself");
        }
        // TODO: check if time passing between pause/unpause is an issue
        // (next tick, the "tick count" will jump forward and maybe Doom won't handle that well)
        if (paused)
            _runningEvent.Reset();
        else
            _runningEvent.Set();
    }

    private void Update()
    {
        CollectKeys();
        if (_frameState == FrameState.GatherInput)
        {
            _frameState = FrameState.DoGameTick;
        }
        // game drew a frame, notify clients
        if (_frameState == FrameState.WaitForDraw)
        {
            _frameState = FrameState.FrameEnd;
            if (_screenBuffer == IntPtr.Zero)
            {
                // theoretically should never happen
                LogSource.LogError("Tried to draw before screen buffer was assigned");
                return;
            }
            Screen.LoadRawTextureData(_screenBuffer, Screen.width * Screen.height * sizeof(uint));
            Screen.Apply();
            _clientManager.OnDrawFrame();
        }
    }

    private readonly HashSet<DoomKey> _pressedKeys = [];
    private readonly HashSet<DoomKey> _heldKeys = [];
    private readonly HashSet<DoomKey> _releasedKeys = [];
    private void CollectKeys()
    {
        if (!Input.anyKey && _heldKeys.Count > 0)
        {
            _releasedKeys.AddRange(_heldKeys);
            return;
        }

        // 6/10 on the funny scale
        // but there are a shitload of keys checked in the C code that aren't in enums at all
        // and i cba to redefine them all
        _pressedKeys.AddRange(Encoding.ASCII.GetBytes(Input.inputString)
            .Cast<DoomKey>()
            .Where(k => !_heldKeys.Contains(k)));

        foreach ((DoomKey doomKey, KeyCode unityKey) in KeyCodeConverter.GetAllKeys())
        {
            bool isDown = Input.GetKey(unityKey);
            if (isDown)
            {
                if (!_heldKeys.Contains(doomKey))
                {
                    LogSource.LogWarning($"CollectKeys pressed {doomKey}");
                    _pressedKeys.Add(doomKey);
                }
            }
            else
            {
                if (_pressedKeys.Contains(doomKey))
                {
                    // alternate bind, duplicate keys, etc.
                    // otherwise it counts as pressed and released in the same tick and therefore the input gets dropped
                    continue;
                }
                if (_heldKeys.Contains(doomKey))
                {
                    LogSource.LogWarning($"CollectKeys released {doomKey}");
                    _releasedKeys.Add(doomKey);
                }
            }
        }
    }
}
