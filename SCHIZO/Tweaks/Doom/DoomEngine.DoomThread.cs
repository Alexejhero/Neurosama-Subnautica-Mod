using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;
partial class DoomEngine
{
    private readonly Stopwatch _gameClock = new();
    private readonly Thread _doomThread = new(StartDoom_);
    private readonly ManualResetEventSlim _runningEvent = new(true);
    private readonly ManualResetEventSlim _inputEvent = new(false);
    private readonly ManualResetEventSlim _drawEvent = new(false);
    private static Thread _mainThread;
    private static int _mainThreadId;

    private static readonly string[] _launchArgs =
    [
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "doomgeneric.dll")
    ];

    private enum FrameState
    {
        FrameStart,
        GatherInput,
        DoGameTick,
        WaitForDraw,
        FrameEnd,
    }
    private FrameState _frameState;

    private static void StartDoom_() => Instance.StartDoom();
    private void StartDoom()
    {
        if (IsStarted)
        {
            LogSource.LogWarning("Tried to start doom more than once");
            return;
        }
        _gameClock.Start();
        Stopwatch sw = Stopwatch.StartNew();
        DoomNative.Start(new()
        {
            Init = Doom_Init,
            DrawFrame = Doom_DrawFrame,
            Sleep = Doom_Sleep,
            GetTicksMillis = Doom_GetTicksMillis,
            GetKey = Doom_GetKey,
            GetMouse = Doom_GetMouse,
            SetWindowTitle = Doom_SetWindowTitle,
            Exit = Doom_Exit,
            Log = Doom_Log,
            // TODO Sound
        }, _launchArgs.Length, _launchArgs);
        sw.Stop();
        StartupTime = (float) sw.Elapsed.TotalMilliseconds;
        LogSource.LogWarning($"Startup took: {StartupTime:n}ms");
        IsStarted = true;
        if (IsOnUnityThread())
        {
            LogSource.LogError("Should not be on Unity thread here");
            return;
        }
        Application.quitting += _doomThread.Abort;
        DoomLoop();
    }

    private void DoomLoop()
    {
        while (true)
        {
            _runningEvent.Wait();
            switch (_frameState)
            {
                case FrameState.FrameStart:
                    _frameState = FrameState.GatherInput;
                    _inputEvent.Reset();
                    break;
                case FrameState.GatherInput:
                    // wait for unity side
                    _inputEvent.Wait();
                    break;
                case FrameState.DoGameTick:
                    DoomNative.Tick();
                    break;
                case FrameState.WaitForDraw:
                    _drawEvent.Wait();
                    break;
                case FrameState.FrameEnd:
                    _clientManager.OnTick();
                    _frameState = FrameState.FrameStart;
                    break;
            }
        }
    }

    private void Doom_Init(int resX, int resY)
    {
        LogSource.LogWarning($"Init {resX}x{resY}");
        //Screen.width = resX;
        //Screen.height = resY;
        Screen.Resize(resX, resY, TextureFormat.BGRA32, false);
        Sprite = Sprite.Create(Screen, new Rect(0, 0, resX, resY), new Vector2(0.5f, 0.5f));
        _clientManager.OnInit();
    }

    private uint Doom_GetTicksMillis()
    {
        CurrentTick = (int) _gameClock.ElapsedMilliseconds;
        //LogSource.LogDebug($"GetTicksMillis {CurrentTick}");
        return (uint) CurrentTick;
    }

    private void Doom_SetWindowTitle(string title)
    {
        LogSource.LogWarning($"SetWindowTitle {title}");
        WindowTitle = title;
        _clientManager.OnWindowTitleChanged(title);
    }

    private IntPtr _screenBuffer;
    private void Doom_DrawFrame(IntPtr screenBuffer, int bufferBytes)
    {
        // special logic for first ever drawn frame
        if (_screenBuffer == IntPtr.Zero)
        {
            LogSource.LogDebug($"DrawFrame (first) {screenBuffer:x}");
            if (screenBuffer == IntPtr.Zero) throw new ArgumentNullException(nameof(screenBuffer));
            _screenBuffer = screenBuffer;
            LogSource.LogDebug($"DrawFrame (first) {screenBuffer:x} assigned");
            return;
        }
        // only draw after game tick starts
        // (we already have the buffer so it doesn't matter when/how many times we get called)
        if (_frameState == FrameState.DoGameTick)
        {
            _frameState = FrameState.WaitForDraw;
            _drawEvent.Reset();
        }
    }

    private void Doom_Sleep(uint millis)
    {
        if (IsOnUnityThread())
        {
            LogSource.LogError($"Should not be on Unity thread in {nameof(Doom_Sleep)}");
            return;
        }
        //LogSource.LogDebug($"Sleep {millis}");
        // millis *= 100;
        Thread.Sleep(TimeSpan.FromMilliseconds(millis));
    }

    private bool Doom_GetKey(out bool pressed, out DoomKey key)
    {
        pressed = default;
        key = default;
        lock (_inputSync)
        {
            foreach (bool isPress in new[] { true, false })
            {
                HashSet<DoomKey> keys = isPress ? _pressedKeys : _releasedKeys;
                if (keys.Count == 0) continue;

                pressed = isPress;
                key = keys.First();
                keys.Remove(key);
                string keyName = Enum.IsDefined(typeof(DoomKey), key)
                    ? key.ToString()
                    : $"'{(char)key}'"; // ascii/limited to byte so it's fine
                //LogSource.LogMessage($"GetKey {keyName} {(isPress ? "press" : "release")} consumed");
                return true;
            }
        }
        //LogSource.LogDebug("GetKey nothing");
        return false;
    }

    private void Doom_GetMouse(out int deltaX, out int deltaY, out int left, out int right, out int middle, out int wheel)
    {
        lock (_inputSync)
        {
            deltaX = (int)Interlocked.Exchange(ref _mouseDeltaX, 0f);
            deltaY = (int)Interlocked.Exchange(ref _mouseDeltaY, 0f);
            wheel = Mathf.Approximately(_mouseWheelDelta, 0f) ? 0 : (int)Mathf.Sign(_mouseWheelDelta);
            _mouseWheelDelta = 0;
            left = Input.GetMouseButton(0) ? 1 : 0;
            right = Input.GetMouseButton(1) ? 1 : 0;
            middle = Input.GetMouseButton(2) ? 1 : 0;
        }
    }

    private void Doom_Exit(int exitCode)
    {
        LogSource.LogWarning($"{nameof(Doom_Exit)} {exitCode}");
        LastExitCode = exitCode;
        IsStarted = false;
        _clientManager.OnExit(exitCode);
    }

    private void Doom_Log(string message)
    {
        LogSource.LogMessage($"(DOOM) {message}");
    }
}
