using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.DataStructures;
using UnityEngine;

namespace SCHIZO.Sounds;

public sealed class SoundCollection : ScriptableObject
{
    private readonly RandomList<string> _sounds = new();

    private readonly List<Coroutine> _runningCoroutines = new();

    public float LastPlay { get; private set; } = -1;

    public static SoundCollection Create(string path, string bus)
    {
        SoundCollection result = CreateInstance<SoundCollection>();
        result.Initialize(path, bus);
        return result;
    }

    public static SoundCollection Combine(params SoundCollection[] collections)
    {
        SoundCollection result = CreateInstance<SoundCollection>();
        foreach (string sound in collections.SelectMany(coll => coll._sounds))
        {
            result._sounds.Add(sound);
        }
        return result;
    }

    private void Initialize(string path, string bus)
    {
        string dirpath = Path.Combine(AssetLoader.AssetsFolder, "sounds", path);

        foreach (string soundFile in Directory.GetFiles(dirpath))
        {
            string id = Guid.NewGuid().ToString();
            RegisterSound(id, soundFile, bus);
            _sounds.Add(id);
        }
    }

    private void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(Player.main.StartCoroutine(coroutine));
    }

    private void RegisterSound(string id, string soundFile, string bus)
    {
        Sound s = CustomSoundHandler.RegisterCustomSound(id, soundFile, bus, AudioUtils.StandardSoundModes_3D);
        RuntimeManager.GetBus(bus).unlockChannelGroup();
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
