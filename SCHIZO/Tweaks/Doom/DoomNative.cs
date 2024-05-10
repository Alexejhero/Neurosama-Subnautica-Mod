using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SCHIZO.Resources;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal static class DoomNative
{
    public const string DLL_NAME = "doomgeneric.dll";
    private static readonly string DIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static readonly string DLL_PATH = Path.Combine(DIR, DLL_NAME);
    private const string DLL_SHA256 = "WacBuUEFMz0jscbJxE1meQIZacaJrycoXwYjXRWAaXk=";
    private static bool _dropped;

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
            ValidateCallback(Init);
            ValidateCallback(DrawFrame);
            ValidateCallback(Sleep);
            ValidateCallback(GetTicksMillis);
            ValidateCallback(GetKey);
            // GetMouse is optional
            // SetWindowTitle is optional
            ValidateCallback(Exit);
            // Log is optional
        }
        private readonly void ValidateCallback(Delegate callback, [CallerArgumentExpression(nameof(callback))] string name = "")
        {
            if (callback is null)
                throw new InvalidOperationException($"Required callback {name} is missing");
        }
    }
    private static Vector2Int _res;
    private static bool _created;

    // keep them pinned
    private static Callbacks _callbacks;
    private static IntPtr _callbackPtr;

    public static void Start(Callbacks callbacks, params string[] args)
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
            // womp womp, spread needs spans (-1h checking everything over like 3 times bc no exception gets logged smile)
            // string[] argv = [Path.GetFullPath(DLL_PATH), ..args];
            string[] argv = args.Prepend(Path.GetFullPath(DLL_PATH)).ToArray();
            NativeCreate(argv.Length, argv);
        }
    }
    public static bool CheckDll()
    {
        Drop();
        if (!File.Exists(DLL_PATH))
            return false;

        string hash;
        using (SHA256Managed hasher = new())
        using (FileStream fs = File.OpenRead(DLL_PATH))
            hash = Convert.ToBase64String(hasher.ComputeHash(fs));

        if (hash != DLL_SHA256)
        {
            DoomEngine.LogWarning($"{DLL_NAME} checksum does not match ({hash} != {DLL_SHA256})");
            return false;
        }
        return true;
    }

    private static void Drop()
    {
        if (!_dropped)
        {
            File.WriteAllBytes(DLL_PATH, ResourceManager.GetEmbeddedBytes(DLL_NAME, false));
            File.WriteAllBytes(Path.Combine(DIR, "DOOM1.WAD"), ResourceManager.GetEmbeddedBytes("DOOM1.WAD", false));
            Environment.SetEnvironmentVariable("DOOMWADDIR", DIR);
            _dropped = true;
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
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetCallbacks")]
    private static extern void NativeSetCallbacks(IntPtr callbacks);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Create")]
    private static extern void NativeCreate(int argc, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPStr)] string[] argv);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Tick")]
    private static extern void NativeTick();
}
