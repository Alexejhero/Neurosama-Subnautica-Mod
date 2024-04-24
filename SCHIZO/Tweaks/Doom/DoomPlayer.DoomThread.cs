using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;
partial class DoomPlayer
{
    private readonly Stopwatch _gameClock = new();
    private readonly Thread _doomThread = new(StartDoom_);
    private readonly ManualResetEventSlim _runningEvent = new(true);
    private static int _mainThreadId;

    private static readonly string[] _launchArgs = ["doomgeneric.dll"];

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
        _launchArgs[0] = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "doomgeneric.dll"
        );
        _gameClock.Start();
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
            Log = Doom_Log,
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
                    break;
                case FrameState.GatherInput:
                    // wait for unity side
                    if (!Thread.Yield())
                        Thread.Sleep(1);
                    break;
                case FrameState.DoGameTick:
                    DoomNative.Tick();
                    break;
                case FrameState.WaitForDraw:
                    // wait for unity side
                    if (!Thread.Yield())
                        Thread.Sleep(1);
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
            _frameState = FrameState.WaitForDraw;
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

        if (_pressedKeys.Count > 0)
        {
            pressed = true;
            key = _pressedKeys.First();
            _pressedKeys.Remove(key);
            _heldKeys.Add(key);
            LogSource.LogMessage($"GetKey {key} pressed");
            return true;
        }
        if (_releasedKeys.Count > 0)
        {
            key = _releasedKeys.First();
            _releasedKeys.Remove(key);
            _heldKeys.Remove(key);
            LogSource.LogMessage($"GetKey {key} released");
            return true;
        }
        //LogSource.LogDebug("GetKey nothing");
        return false;
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
