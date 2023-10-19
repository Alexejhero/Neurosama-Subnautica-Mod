using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.DataStructures;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds;

[Serializable]
[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Serialization")]
public sealed class FMODSoundCollection
{
    [SerializeField] private string _bus;
    [SerializeField] private List<string> _sounds = new();

    private RandomList<string> _randomSounds;
    private List<Coroutine> _runningCoroutines;

    public FMODSoundCollection(BaseSoundCollection soundCollection, string bus)
    {
        _bus = bus;

        foreach (AudioClip audioClip in soundCollection.GetSounds())
        {
            string id = Guid.NewGuid().ToString();
            RegisterSound(id, audioClip);
            _sounds.Add(id);
        }
    }

    private void Initialize()
    {
        if (_randomSounds is { Count: > 0 }) return;

        _randomSounds = new RandomList<string>();
        _runningCoroutines = new List<Coroutine>();
        _randomSounds.AddRange(_sounds);
    }

    public float LastPlay { get; private set; } = -1;

    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(CoroutineHost.StartCoroutine(coroutine));
    }

    private void RegisterSound(string id, AudioClip audioClip)
    {
        Sound s = CustomSoundHandler.RegisterCustomSound(id, audioClip, _bus, AudioUtils.StandardSoundModes_3D);
        RuntimeManager.GetBus(_bus).unlockChannelGroup();
        s.set3DMinMaxDistance(1, 30);
    }

    public void CancelAllDelayed()
    {
        Initialize();

        foreach (Coroutine c in _runningCoroutines)
        {
            CoroutineHost.StopCoroutine(c);
        }

        _runningCoroutines.Clear();
    }

    public void Play2D(float delay = 0) => Play(null, delay);

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        Initialize();
        if (CONFIG.DisableAllNoises) return;

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
            CustomSoundHandler.TryPlayCustomSound(sound, out Channel channel);
            channel.set3DLevel(0);
        }
    }
}
