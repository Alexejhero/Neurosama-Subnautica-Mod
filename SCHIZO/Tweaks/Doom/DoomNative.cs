using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal static class DoomNative
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InitCallback(int resX, int resY);
    // note: this actually refers to a uint[] buffer but we interpret it as byte[] for Unity texture data
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DrawFrameCallback(IntPtr screenBuffer, int bufferBytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SleepCallback(uint millis);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint GetTicksMillisCallback();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool GetKeyCallback(out bool pressed, out DoomKey doomKey);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetMouseCallback(out int deltax, out int deltay, out bool left, out bool right, out bool middle, out int wheel);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SetWindowTitleCallback([MarshalAs(UnmanagedType.LPStr)] string title);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Exit(int exitCode);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log([In, MarshalAs(UnmanagedType.LPStr)] string message);

    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public InitCallback Init;
        public DrawFrameCallback DrawFrame;
        public SleepCallback Sleep;
        public GetTicksMillisCallback GetTicksMillis;
        public GetKeyCallback GetKey;
        public GetMouseCallback GetMouse;
        public SetWindowTitleCallback SetWindowTitle;
        public Exit Exit;
        public Log Log;

        public readonly void Validate()
        {
            if (Init is null)
                throw new InvalidOperationException($"Required callback {nameof(Init)} is missing");
            if (DrawFrame is null)
                throw new InvalidOperationException($"Required callback {nameof(DrawFrame)} is missing");
            if (Sleep is null)
                throw new InvalidOperationException($"Required callback {nameof(Sleep)} is missing");
            if (GetTicksMillis is null)
                throw new InvalidOperationException($"Required callback {nameof(GetTicksMillis)} is missing");
            if (GetKey is null)
                throw new InvalidOperationException($"Required callback {nameof(GetKey)}) is missing");
            // GetMouse is optional
            // SetWindowTitle is optional
            if (Exit is null)
                throw new InvalidOperationException($"Required callback {nameof(Exit)}) is missing");
            // Log is optional
        }
    }
    private static Vector2Int _res;
    private static bool _created;

    // keep them pinned
    private static Callbacks _callbacks;
    private static IntPtr _callbackPtr;

    public static void Start(Callbacks callbacks, int argc, string[] argv)
    {
        callbacks.Validate();
        // technically the C side is designed to only get "created" once
        // i've done the best i could in terms of freeing/skipping reallocs, but... you're on your own
        _callbacks = WrapCallbacks(callbacks);
        bool firstStart = _callbackPtr == IntPtr.Zero;
        if (firstStart)
            _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
        Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        NativeSetCallbacks(_callbackPtr);
        if (_created)
        {
            // maybe throw instead?
            callbacks.Init(_res.x, _res.y);
        }
        else
        {
            NativeCreate(argc, argv);
        }
    }
    public static void Tick() => NativeTick();

    private static Callbacks WrapCallbacks(Callbacks callbacks)
    {
        return callbacks with
        {
            Init = WrapInitCallback(callbacks.Init),
            Exit = WrapExitCallback(callbacks.Exit),
        };
    }

    private static InitCallback WrapInitCallback(InitCallback inner)
    {
        return (resX, resY) =>
        {
            _res = new(resX, resY);
            inner(resX, resY);
            _created = true;
        };
    }

    private static Exit WrapExitCallback(Exit inner)
    {
        return (code) =>
        {
            inner(code);
            if (code == 0) return;
            // destroy and free
            _res = default;
            _created = false;
            Marshal.DestroyStructure<Callbacks>(_callbackPtr);
            Marshal.FreeHGlobal(_callbackPtr);
            _callbackPtr = IntPtr.Zero;
        };
    }

    // our interface
    [DllImport("doomgeneric.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetCallbacks")]
    private static extern void NativeSetCallbacks(IntPtr callbacks);

    [DllImport("doomgeneric.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Create")]
    private static extern void NativeCreate(int argc, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPStr)] string[] argv);

    [DllImport("doomgeneric.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Tick")]
    private static extern void NativeTick();
}
