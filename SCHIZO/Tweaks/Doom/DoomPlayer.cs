using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal class DoomPlayer : MonoBehaviour
{
    public Texture2D Screen { get; private set; }
    public Vector2 ScreenResolution => new(Screen.width, Screen.height);

    public Action OnTick;
    public Action OnDrawFrame;

    public bool IsStarted { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsSleeping => !_doomThreadSync.IsSet;
    public string WindowTitle { get; private set; }

    public float StartupTime { get; private set; }
    public int LastExitCode { get; private set; }

    private Thread _doomThread;
    private ManualResetEventSlim _doomThreadSync;
    private float _sleepTimeRemaining;

    private IEnumerator Start()
    {
        Screen = new Texture2D(0, 0, TextureFormat.RGBA32, false);
        _doomThreadSync = new(false);
        _doomThread = new Thread(() =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            DoomNative.Start(new()
            {
                Init = Doom_Init,
                DrawFrame = Doom_DrawFrame,
                Sleep = Doom_Sleep,
                GetTicksMillis = Doom_GetTicksMillis,
                GetKey = Doom_GetKey,
                SetWindowTitle = Doom_SetWindowTitle,
                Exit = Doom_Exit,
            }, 1, ["@log.txt"]);
            sw.Stop();
            StartupTime = (float) sw.Elapsed.TotalSeconds;
            LOGGER.LogWarning($"Startup time: {StartupTime}");
            IsStarted = true;
        });
        _doomThread.Start();
        yield return new WaitUntil(() => IsStarted);
        yield return StartCoroutine(Doom_Loop());
    }

    private void OnEnable()
    {
        IsRunning = true;
    }

    private void OnDisable()
    {
        // TODO: check if time passing between disable/enable is an issue
        // (next tick, the "tick count" will jump forward and maybe Doom won't handle that well)
        IsRunning = false;
    }
    private void Doom_Init(int resX, int resY)
    {
        LOGGER.LogWarning($"Init {resX}x{resY}");
        Screen.Resize(resX, resY);
    }

    private uint Doom_GetTicksMillis()
    {
        uint millis = (uint) Mathf.FloorToInt(Time.realtimeSinceStartup * 1000);
        // LOGGER.LogDebug($"GetTicksMillis {millis}");
        return millis;
    }

    private void Doom_SetWindowTitle(string title)
    {
        LOGGER.LogWarning($"SetWindowTitle {title}");
        WindowTitle = title;
    }

    private IEnumerator Doom_Loop()
    {
        while (true)
        {
            if (!(IsStarted && IsRunning))
            {
                LOGGER.LogMessage("Not running");
                yield return null;
                yield break;
            }
            if (IsSleeping)
            {
                LOGGER.LogWarning($"Sleeping for {_sleepTimeRemaining}");
                yield return new WaitForSecondsRealtime(_sleepTimeRemaining);
                _doomThreadSync.Set();
            }
            yield return Tick();
        }
    }

    private IEnumerator Tick()
    {
        CollectKeys();
        LOGGER.LogMessage("Tick");
        // uhhhhh we need to somehow put a tick on the thread (and then come back to this one)
        Task tick = Task.Run(DoomNative.Tick);
        yield return new WaitUntil(() => tick.IsCompleted);
        OnTick?.Invoke();
    }

    private void Doom_DrawFrame(byte[] screenBuffer, int bufferBytes)
    {
        //LOGGER.LogDebug($"DrawFrame {screenBuffer} {bufferBytes}");
        CheckNullArgument(screenBuffer, nameof(screenBuffer));
        Screen.LoadRawTextureData(screenBuffer);
        OnDrawFrame?.Invoke();
    }

    private void Doom_Sleep(uint millis)
    {
        //LOGGER.LogDebug($"Sleep {millis}");
        _doomThreadSync.Reset();
        _doomThreadSync.Wait();
    }

    private readonly HashSet<DoomKey> _pressedKeys = [];
    // track held keys to be able to notify on release
    private readonly HashSet<DoomKey> _heldKeys = [];
    private readonly HashSet<DoomKey> _releasedKeys = [];
    private void CollectKeys()
    {
        LOGGER.LogDebug("CollectKeys");
        _pressedKeys.Clear();
        _releasedKeys.Clear();
        // we were holding some keys and now we're not holding any
        // ergo, those keys were released
        if (!Input.anyKey && _heldKeys.Count > 0)
        {
            LOGGER.LogWarning($"CollectKeys releasing {_heldKeys.Count} held");
            _releasedKeys.AddRange(_heldKeys);
            _heldKeys.Clear();
            return;
        }

        // this is (presumably) only called once per doom frame (35fps)
        foreach ((KeyCode unityKey, DoomKey doomKey) in KeyCodeConverter.GetAllKeys())
        {
            bool isDown = Input.GetKey(unityKey);
            if (isDown)
            {
                if (!_heldKeys.Contains(doomKey))
                    _pressedKeys.Add(doomKey);
                _heldKeys.Add(doomKey);
            }
            else
            {
                if (_heldKeys.Contains(doomKey))
                    _releasedKeys.Add(doomKey);
                _heldKeys.Remove(doomKey);
            }
        }
    }

    private bool Doom_GetKey(out bool pressed, out DoomKey key)
    {
        pressed = default;
        key = default;

        if (_pressedKeys.Count > 0)
        {
            pressed = true;
            key = _pressedKeys.First();
            _pressedKeys.Remove(key);
            LOGGER.LogWarning($"GetKey {key} pressed");
            return true;
        }
        if (_releasedKeys.Count > 0)
        {
            LOGGER.LogWarning($"GetKey {key} released");
            key = _releasedKeys.First();
            _releasedKeys.Remove(key);
            return true;
        }
        LOGGER.LogDebug("GetKey nothing");
        return false;
    }

    private void Doom_Exit(int exitCode)
    {
        LOGGER.LogWarning($"Exit {exitCode}");
        LastExitCode = exitCode;
        IsStarted = false;
    }
}
