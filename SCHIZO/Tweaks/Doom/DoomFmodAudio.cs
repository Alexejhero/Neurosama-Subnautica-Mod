using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using Nautilus.Utility;
using UnityEngine;
using SfxData = SCHIZO.Tweaks.Doom.DoomAudioNative.SoundModule.SfxData;
using SfxCallbacks = SCHIZO.Tweaks.Doom.DoomAudioNative.SoundModule.Callbacks;
using MusCallbacks = SCHIZO.Tweaks.Doom.DoomAudioNative.MusicModule.Callbacks;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using JetBrains.Annotations;

namespace SCHIZO.Tweaks.Doom;

internal static class DoomFmodAudio
{
    private static readonly Dictionary<IntPtr, Sound> _sfx = [];
    private static Bus _sfxBus;
    private static Bus _musBus;
    private static Sound _song;
    private static EventInstance _playingSong;

    private static readonly FMOD.System _coreSystem = RuntimeManager.CoreSystem;
    private static readonly FMOD.Studio.System _studioSystem = RuntimeManager.StudioSystem;
    private const string DOOM_BUS_PREFIX = "bus:/master/SFX_for_pause/PDA_pause/all/SFX/Doom";
    private const string SFX_EVENT = "event:/SCHIZO/doom/sfx";
    private const string MUS_EVENT = "event:/SCHIZO/doom/mus";

    private const string MUTE_INGAME_MUSIC_SNAPSHOT = "snapshot:/SCHIZO/mute ingame music";
    private static EventInstance _muteSnapshot;

    /// <summary>
    /// Emit sounds as though coming from this transform's position.<br/>
    /// If <see langword="null"/>, plays in 2D.
    /// </summary>
    public static Transform Emitter { get; set; }

    #region sfx
    public static SfxCallbacks SfxCallbacks()
    {
        return new()
        {
            Init = InitSfx,
            Shutdown = ShutdownSfx,
            Update = UpdateSfx,
            UpdateSoundParams = UpdateSfxParams,
            StartSound = StartSfx,
            StopSound = StopSfx,
            IsPlaying = IsPlayingSfx,
            CacheSounds = CacheSfx,
        };
    }
    public static bool InitSfx()
    {
        return _studioSystem.getBus($"{DOOM_BUS_PREFIX}/SFX", out _sfxBus).CheckResult();
    }
    public static void ShutdownSfx()
    {
        _sfxBus.stopAllEvents(STOP_MODE.IMMEDIATE);
        foreach (Sound snd in _sfx.Values)
        {
            snd.release();
        }
        _sfx.Clear();
    }
    public static void UpdateSfx()
    {
    }
    public static void UpdateSfxParams(IntPtr handle, int vol, int sep)
    {
        EventInstance evt = new(handle);
        evt.setVolume(vol / 254f);
        evt.setParameterByName("Pan", sep / 254f);
    }
    public static IntPtr StartSfx(IntPtr sfxInfo, int _, int vol, int sep)
    {
        EventInstance evt = RuntimeManager.CreateInstance(SFX_EVENT);
        evt.setUserData(sfxInfo).CheckResult();
        evt.setCallback(SfxEventCallback, EVENT_CALLBACK_TYPE.ALL).CheckResult();
        evt.setParameterByName("3D", Emitter ? 1 : 0);
        if (Emitter)
        {
            evt.set3DAttributes(Emitter.To3DAttributes());
            DoomEngine.Instance.RunOnUnityThread(() => RuntimeManager.AttachInstanceToGameObject(evt, Emitter));
        }

        evt.setVolume(0);
        UpdateSfxParams(evt.handle, vol, sep);
        evt.start().CheckResult();
        evt.release().CheckResult();
        return evt.handle;
    }
    public static void StopSfx(IntPtr handle)
    {
        EventInstance evt = new(handle);
        evt.stop(STOP_MODE.ALLOWFADEOUT);
        DoomEngine.Instance.RunOnUnityThread(() => RuntimeManager.DetachInstanceFromGameObject(evt));
    }
    public static bool IsPlayingSfx(IntPtr handle)
    {
        return RESULT.OK == EventInstance.FMOD_Studio_EventInstance_GetPaused(handle, out bool paused)
            && !paused;
    }
    public static void CacheSfx(SfxData[] sounds, int num_sounds)
    {
        // noop because we don't have samples here (blame the C side)
        /*
        foreach (DoomAudioNative.SoundModule.SfxData data in sounds)
        {
            Sound snd = MakeSound(data);
            if (snd.hasHandle())
                _sfx[data.data] = snd;
        }
        */
    }
    #endregion

    #region music
    public static MusCallbacks MusicCallbacks()
    {
        return new()
        {
            Init = InitMusic,
            Shutdown = ShutdownMusic,
            SetVolume = SetMusicVolume,
            Pause = PauseMusic,
            Resume = ResumeMusic,
            RegisterSong = RegisterSong,
            UnRegisterSong = UnRegisterSong,
            PlaySong = PlaySong,
            StopSong = StopSong,
            IsPlaying = IsPlayingMusic,
            Poll = PollMusic,
        };
    }
    public static bool InitMusic()
    {
        return _studioSystem.getBus($"{DOOM_BUS_PREFIX}/Music", out _musBus).CheckResult();
    }
    public static void ShutdownMusic()
    {
        _musBus.stopAllEvents(STOP_MODE.IMMEDIATE);
    }
    public static void SetMusicVolume(int vol)
    {
        _musBus.setVolume(vol / 254f);
    }
    public static void PauseMusic()
    {
        _playingSong.setPaused(true);
    }
    public static void ResumeMusic()
    {
        _playingSong.setPaused(false);
    }
    public static IntPtr RegisterSong(IntPtr data, int length)
    {
        if (_song.hasHandle())
        {
            DoomEngine.LogWarning("Already have song, gonna unregister");
            UnRegisterSong(_song.handle);
        }
        CREATESOUNDEXINFO exinfo = new()
        {
            cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>(),
            userdata = data,
            suggestedsoundtype = SOUND_TYPE.MIDI,
            length = (uint) length
        };
        RESULT res = _coreSystem.createSound(data, MODE.OPENMEMORY_POINT | MODE.LOOP_NORMAL | AudioUtils.StandardSoundModes_3D, ref exinfo, out _song);
        if (res != RESULT.OK)
        {
            DoomEngine.LogWarning($"Failed to create song - {res}");
            return IntPtr.Zero;
        }
        return _song.handle;
    }

    public static void UnRegisterSong(IntPtr handle)
    {
        if (_song.handle == handle)
        {
            _song.handle = IntPtr.Zero;
            _playingSong.stop(STOP_MODE.IMMEDIATE);
        }
        Sound.FMOD5_Sound_Release(handle);
    }

    public static void PlaySong(IntPtr handle, bool looping)
    {
        if (handle == IntPtr.Zero)
        {
            DoomEngine.LogWarning("Tried to play null song");
            return;
        }
        if (_song.handle != handle)
        {
            StopSong();
            UnRegisterSong(_song.handle);
            _song = new(handle);
        }
        _song.setMode(looping ? MODE.LOOP_NORMAL : MODE.LOOP_OFF);
        _playingSong = RuntimeManager.CreateInstance(MUS_EVENT);
        _playingSong.setUserData(_song.handle).CheckResult();
        _playingSong.setCallback(MusEventCallback, EVENT_CALLBACK_TYPE.ALL).CheckResult();
        _playingSong.start().CheckResult();
        _playingSong.release().CheckResult();
    }
    public static void StopSong()
    {
        _playingSong.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public static bool IsPlayingMusic()
    {
        return RESULT.OK == _playingSong.getPaused(out bool paused)
            && !paused;
    }
    public static void PollMusic()
    {
        if (!_playingSong.isValid()) return;

        if (!DoomEngine.Instance.IsRunning)
            _playingSong.setPaused(true);

        _playingSong.setParameterByName("3D", Emitter ? 1 : 0).CheckResult();
        if (Emitter)
        {
            _playingSong.set3DAttributes(Emitter.To3DAttributes());
        }
    }
    #endregion

    private static RESULT SfxEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr paramPtr)
    {
        EventInstance instance = new(instancePtr);
        instance.getUserData(out IntPtr infoPtr);

        PROGRAMMER_SOUND_PROPERTIES param; // maybe one day we'll be able to declare same-named variables inside switch cases
        switch (type)
        {
            case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                param = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(paramPtr);
                SfxData sfxInfo = Marshal.PtrToStructure<SfxData>(infoPtr);

                IntPtr start = sfxInfo.data + 32; // 16 bytes padding before, expanded to 32 "samples"
                // quoting from the docs:
                //  With FMOD_OPENMEMORY_POINT and FMOD_OPENRAW or PCM, if using them together, note that you must pad the data on each side by 16 bytes.
                //  This is so fmod can modify the ends of the data for looping / interpolation / mixing purposes.
                // (https://www.fmod.com/docs/2.01/api/core-api-common.html#fmod_virtual_playfromstart - end of the paragraph just before FMOD_RESULT)
                CREATESOUNDEXINFO exinfo = new()
                {
                    cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>(),
                    userdata = start,
                    defaultfrequency = sfxInfo.sample_rate,
                    length = sfxInfo.num_samples * 2, // bytes
                    format = SOUND_FORMAT.PCM16,
                    suggestedsoundtype = SOUND_TYPE.RAW,
                    numchannels = 1,
                };
                RESULT soundRes = _coreSystem.createSound(start, MODE.OPENMEMORY_POINT | MODE.OPENRAW | AudioUtils.StandardSoundModes_3D, ref exinfo, out Sound sound);
                if (soundRes != RESULT.OK)
                {
                    DoomEngine.LogWarning($"Failed to create sound: {soundRes}");
                    return soundRes;
                }
                param.sound = sound.handle;
                param.subsoundIndex = -1;
                Marshal.StructureToPtr(param, paramPtr, false);
                break;
            case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                param = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(paramPtr);
                Sound.FMOD5_Sound_Release(param.sound);
                break;
        }
        return RESULT.OK;
    }
    private static RESULT MusEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr paramPtr)
    {
        EventInstance instance = new(instancePtr);
        instance.getUserData(out IntPtr soundPtr);

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                PROGRAMMER_SOUND_PROPERTIES param = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(paramPtr);
                param.sound = soundPtr;
                param.subsoundIndex = -1;
                Marshal.StructureToPtr(param, paramPtr, false);
                break;
            case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                param = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(paramPtr);
                Sound.FMOD5_Sound_Release(param.sound);
                break;
        }
        return RESULT.OK;
    }

    [UsedImplicitly] // for debugging
    private static string ToHex(this IntPtr ptr) => ptr.ToInt64().ToString("X");

    public static void ToggleIngameMusicMute(bool mute)
    {
        if (!_muteSnapshot.isValid())
        {
            _muteSnapshot = RuntimeManager.CreateInstance(MUTE_INGAME_MUSIC_SNAPSHOT);
            _muteSnapshot.start();
            _muteSnapshot.release();
        }
        _muteSnapshot.setParameterByName("Intensity", mute ? 100 : 0);
    }
}
