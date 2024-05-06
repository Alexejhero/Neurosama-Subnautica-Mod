using System;
using System.Runtime.InteropServices;
using FMOD;

namespace SCHIZO.Tweaks.Doom;

internal static class DoomAudioNative
{
    public abstract class SoundModule
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8, Size = 16)]
        public struct SfxData
        {
            public ushort sample_rate;
            public uint num_samples;
            public IntPtr data; // short*
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool InitCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShutdownCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateSoundParamsCallback(IntPtr channel, int vol, int sep);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr StartSoundCallback([In, MarshalAs(UnmanagedType.LPStruct)] SfxData sfxinfo, int channel, int vol, int sep);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void StopSoundCallback(IntPtr channel);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool IsPlayingCallback(IntPtr channel);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CacheSoundsCallback([In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.LPStruct)] SfxData[] sounds, int num_sounds);

        [StructLayout(LayoutKind.Sequential)]
        public struct Callbacks
        {
            public InitCallback Init;
            public ShutdownCallback Shutdown;
            public UpdateCallback Update;
            public UpdateSoundParamsCallback UpdateSoundParams;
            public StartSoundCallback StartSound;
            public StopSoundCallback StopSound;
            public IsPlayingCallback IsPlaying;
            public CacheSoundsCallback CacheSounds;
        }

        // keep them pinned
        internal static Callbacks _callbacks;
        internal static IntPtr _callbackPtr;

        public static void Assign(Callbacks callbacks)
        {
            _callbacks = callbacks;
            bool firstStart = _callbackPtr == IntPtr.Zero;
            if (firstStart)
                _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
            Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        }
    }
    public static class MusicModule
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool InitCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ShutdownCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetVolumeCallback(int volume);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PauseCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ResumeCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr RegisterSongCallback(IntPtr data, int len);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnRegisterSongCallback(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PlaySongCallback(IntPtr handle, bool looping);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void StopSongCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool IsPlayingCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PollCallback();

        [StructLayout(LayoutKind.Sequential)]
        public struct Callbacks
        {
            public InitCallback Init;
            public ShutdownCallback Shutdown;
            public SetVolumeCallback SetVolume;
            public PauseCallback Pause;
            public ResumeCallback Resume;
            public RegisterSongCallback RegisterSong;
            public UnRegisterSongCallback UnRegisterSong;
            public PlaySongCallback PlaySong;
            public StopSongCallback StopSong;
            public IsPlayingCallback IsPlaying;
            public PollCallback Poll;
        }

        internal static Callbacks _callbacks;
        internal static IntPtr _callbackPtr;

        public static void Assign(Callbacks callbacks)
        {
            _callbacks = callbacks;
            bool firstStart = _callbackPtr == IntPtr.Zero;
            if (firstStart)
                _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
            Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        }
    }
    public static void SetAudioCallbacks(SoundModule.Callbacks sndCallbacks, MusicModule.Callbacks musCallbacks)
    {
        SoundModule.Assign(sndCallbacks);
        MusicModule.Assign(musCallbacks);

        NativeSetAudioCallbacks(SoundModule._callbackPtr, MusicModule._callbackPtr);
    }
    [DllImport("doomgeneric.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetAudioCallbacks")]
    private static extern void NativeSetAudioCallbacks(IntPtr sndCallbacks, IntPtr musCallbacks);
}
