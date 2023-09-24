using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Unity.Sounds;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds;

public sealed class SoundPlayer
{
    private readonly string _bus;

    private readonly List<string> _sounds = new();
    private readonly List<Coroutine> _runningCoroutines = new();

    public SoundPlayer(BaseSoundCollection soundCollection, string bus)
    {
        _bus = bus;

        foreach (AudioClip audioClip in soundCollection.GetSounds())
        {
            string id = Guid.NewGuid().ToString();
            RegisterSound(id, audioClip);
            _sounds.Add(id);
        }
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
        foreach (Coroutine c in _runningCoroutines)
        {
            GameInput.instance.StopCoroutine(c);
        }

        _runningCoroutines.Clear();
    }

    public void Play2D(float delay = 0) => Play(null, delay);

    public void Play(FMOD_CustomEmitter emitter = null, float delay = 0)
    {
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

        if (emitter)
        {
            emitter.SetAsset(AudioUtils.GetFmodAsset(_sounds.GetRandom()));
            emitter.Play();
        }
        else
        {
            CustomSoundHandler.TryPlayCustomSound(_sounds.GetRandom(), out Channel channel);
            channel.set3DLevel(0);
        }
    }
}
