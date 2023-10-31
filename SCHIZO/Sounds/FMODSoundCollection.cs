using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.DataStructures;
using SCHIZO.Resources;
using SCHIZO.Sounds.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds;

public sealed class FMODSoundCollection
{
    private static readonly Dictionary<string, FMODSoundCollection> _cache = new();

    private readonly string _busName;
    private readonly List<string> _sounds = new();

    private bool _ready;
    private RandomList<string> _randomSounds;
    private List<Coroutine> _runningCoroutines;

    public static FMODSoundCollection For(SoundCollection soundCollection, string bus)
    {
        int instanceId = soundCollection.GetInstanceID();

        if (_cache.TryGetValue(instanceId + bus, out FMODSoundCollection cached)) return cached;
        return _cache[instanceId + bus] = new FMODSoundCollection(soundCollection, bus);
    }

    private FMODSoundCollection(SoundCollection soundCollection, string bus)
    {
        _busName = bus;
        CoroutineHost.StartCoroutine(LoadSounds(soundCollection));
    }

    private IEnumerator LoadSounds(SoundCollection soundCollection)
    {
        Bus bus = RuntimeManager.GetBus(_busName);

        foreach (AudioClip audioClip in soundCollection.GetSounds())
        {
            string id = Guid.NewGuid().ToString();
            RegisterSound(id, audioClip, bus);
            _sounds.Add(id);

            yield return null;
        }

        _ready = true;
    }

    private bool Initialize()
    {
        if (!_ready) return false;
        if (_randomSounds is {Count: > 0}) return true;

        _randomSounds = new RandomList<string>();
        _runningCoroutines = new List<Coroutine>();
        _randomSounds.AddRange(_sounds);

        return true;
    }

    public float LastPlay { get; private set; } = -1;

    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(CoroutineHost.StartCoroutine(coroutine));
    }

    private void RegisterSound(string id, AudioClip audioClip, Bus bus)
    {
        Sound s = CustomSoundHandler.RegisterCustomSound(id, audioClip, bus, AudioUtils.StandardSoundModes_3D);
        bus.unlockChannelGroup();
        s.set3DMinMaxDistance(1, 30);
    }

    public void CancelAllDelayed()
    {
        if (!Initialize()) return;

        foreach (Coroutine c in _runningCoroutines)
        {
            CoroutineHost.StopCoroutine(c);
        }

        _runningCoroutines.Clear();
    }

    public void Play2D(float delay = 0) => Play(null, delay);

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        if (!Initialize()) return;
        if (Assets.Options_DisableAllSounds.Value) return;

        if (delay <= 0)
        {
            PlaySound(emitter);
            return;
        }

        StartSoundCoroutine(PlayWithDelay(delay));
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlaySound(emitter);
        }
    }

    private void PlaySound(FMOD_CustomEmitter emitter = null)
    {
        LastPlay = Time.time;

        string sound = _sounds.GetRandom();

        if (emitter)
        {
            emitter.SetAsset(AudioUtils.GetFmodAsset(sound));
            emitter.Play();
        }
        else
        {
            // TODO: Play this sound using an Emitter or something (so that it changes based on volume)
            CustomSoundHandler.TryPlayCustomSound(sound, out Channel channel);
            channel.set3DLevel(0);
        }
    }
}
