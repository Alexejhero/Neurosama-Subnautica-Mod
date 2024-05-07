using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;

namespace SCHIZO.Tweaks.Doom;

internal static class DoomFmodAudio
{
    private static readonly Dictionary<IntPtr, Sound> _sfx = [];
    private static Bus _sfxBus;
    private static ChannelGroup _sfxChannels;
    private static Bus _musBus;
    private static ChannelGroup _musChannels;
    private static Sound _song;
    private static Channel _playingSong;

    private static readonly FMOD.System _coreSystem = FMODUnity.RuntimeManager.CoreSystem;
    private static readonly FMOD.Studio.System _studioSystem = FMODUnity.RuntimeManager.StudioSystem;
    private const string DOOM_BUS_PREFIX = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds/Doom";

    #region sfx
    public static DoomAudioNative.SoundModule.Callbacks SfxCallbacks()
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
        if (RESULT.OK != _studioSystem.getBus($"{DOOM_BUS_PREFIX}/SFX", out _sfxBus))
            return false;
        if (!_sfxBus.lockChannelGroup().CheckResult())
        {
            DoomEngine.LogWarning("Failed to lock channel group for sfx");
            return false;
        }
        _studioSystem.flushCommands();
        if (!_sfxBus.getChannelGroup(out _sfxChannels).CheckResult())
        {
            DoomEngine.LogWarning("Failed to get channel group for sfx");
            _sfxBus.unlockChannelGroup();
            return false;
        }
        _sfxChannels.stop();
        return true;
    }
    public static void ShutdownSfx()
    {
        _sfxChannels.stop();
        _sfxChannels.release();
        foreach (Sound snd in _sfx.Values)
        {
            snd.release();
        }
        _sfx.Clear();
        _sfxBus.unlockChannelGroup();
    }
    public static void UpdateSfx()
    {
        // FMOD releases channels automatically
    }
    public static void UpdateSfxParams(IntPtr channelHandle, int vol, int sep)
    {
        Channel channel = new(channelHandle);
        channel.setVolume(vol / 254f);
        channel.setPan(sep / 254f);
    }
    private static bool MakeSound(DoomAudioNative.SoundModule.SfxData data, out Sound sound)
    {
        sound = default;
        if (data.data == IntPtr.Zero)
        {
            DoomEngine.LogWarning("Failed to create sound: data is null");
            return false;
        }
        CREATESOUNDEXINFO exinfo = new()
        {
            cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>(),
            userdata = data.data,
            defaultfrequency = data.sample_rate,
            length = data.num_samples * 2, // bytes
            format = SOUND_FORMAT.PCM16,
            suggestedsoundtype = SOUND_TYPE.RAW,
            numchannels = 1,
        };
        RESULT soundRes = _coreSystem.createSound(data.data, MODE.OPENMEMORY_POINT | MODE.OPENRAW, ref exinfo, out sound);
        if (soundRes != RESULT.OK)
        {
            DoomEngine.LogWarning($"Failed to create sound: {soundRes}");
            return false;
        }
        return true;
    }
    public static IntPtr StartSfx(DoomAudioNative.SoundModule.SfxData data, int channelIndex, int vol, int sep)
    {
        if (!_sfx.TryGetValue(data.data, out Sound sound) && !MakeSound(data, out sound))
        {
            return new(-1);
        }

        RESULT playRes = _coreSystem.playSound(sound, _sfxChannels, false, out Channel channel);
        if (playRes != RESULT.OK)
        {
            DoomEngine.LogWarning($"Failed to play sound: {playRes}");
            return new(-1);
        }
        channel.setPosition(16, TIMEUNIT.PCMBYTES);
        //sound.getLength(out uint length, TIMEUNIT.MS);
        //DoomEngine.LogDebug($"Channel {channelIndex} playing sound {data.data} for {length}ms");
        UpdateSfxParams(channel.handle, vol, sep);
        return channel.handle;
    }
    public static void StopSfx(IntPtr channelHandle)
    {
        Channel.FMOD5_Channel_Stop(channelHandle);
    }
    public static bool IsPlayingSfx(IntPtr channelHandle)
    {
        return RESULT.OK == Channel.FMOD5_Channel_IsPlaying(channelHandle, out bool playing)
            && playing;
    }
    public static void CacheSfx(DoomAudioNative.SoundModule.SfxData[] sounds, int num_sounds)
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
    public static DoomAudioNative.MusicModule.Callbacks MusicCallbacks()
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
        if (RESULT.OK != _studioSystem.getBus($"{DOOM_BUS_PREFIX}/Music", out _musBus))
            return false;
        if (!_musBus.lockChannelGroup().CheckResult())
        {
            DoomEngine.LogWarning("Failed to lock channel group for music");
            return false;
        }
        _studioSystem.flushCommands();
        if (!_musBus.getChannelGroup(out _musChannels).CheckResult())
        {
            DoomEngine.LogWarning("Failed to get channel group for music");
            _musBus.unlockChannelGroup();
            return false;
        }
        return true;
    }
    public static void ShutdownMusic()
    {
        _musChannels.stop();
        _musChannels.release();
        _musBus.unlockChannelGroup();
    }
    public static void SetMusicVolume(int vol)
    {
        _musBus.setVolume(vol / 127f);
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
            UnRegisterSong(_song.handle);
        }
        CREATESOUNDEXINFO exinfo = new()
        {
            cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>(),
            userdata = data,
            suggestedsoundtype = SOUND_TYPE.MIDI,
            length = (uint) length
        };
        RESULT res = _coreSystem.createSound(data, MODE.OPENMEMORY_POINT | MODE.LOOP_NORMAL, ref exinfo, out _song);
        if (res != RESULT.OK)
        {
            DoomEngine.LogWarning($"Failed to create song - {res}");
            return IntPtr.Zero;
        }
        return _song.handle;
    }

    public static void UnRegisterSong(IntPtr handle)
    {
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
        _coreSystem.playSound(_song, _musChannels, false, out _playingSong);
    }
    public static void StopSong()
    {
        _musChannels.stop();
    }

    public static bool IsPlayingMusic()
    {
        return _playingSong.hasHandle()
            && RESULT.OK == _playingSong.isPlaying(out bool playing)
            && playing;
    }
    public static void PollMusic()
    {
        if (!DoomEngine.Instance.IsRunning)
            _playingSong.setPaused(true);
    }
    #endregion
}
