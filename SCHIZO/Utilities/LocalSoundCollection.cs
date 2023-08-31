using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Utilities;

public sealed class LocalSoundCollection
{
    private readonly List<string> _remainingSounds = new();
    private readonly List<string> _playedSounds = new();
    private readonly List<Coroutine> _runningCoroutines = new();

    public float LastPlay { get; private set; } = -1;

    public LocalSoundCollection(string path, string bus)
    {
        string dirpath = Path.Combine(AssetLoader.AssetsFolder, "sounds", path);

        foreach (string soundFile in Directory.GetFiles(dirpath))
        {
            string id = Guid.NewGuid().ToString();
            Sound s = CustomSoundHandler.RegisterCustomSound(id, soundFile, bus, MODE._3D | MODE._3D_LINEARSQUAREROLLOFF);
            s.set3DMinMaxDistance(1, 30);
            _remainingSounds.Add(id);
        }

        _remainingSounds.Shuffle();
    }

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        if (!emitter) return;
        if (Plugin.Config.DisableAllNoises) return;

        if (delay == 0)
        {
            PlaySound(emitter);
            return;
        }

        Coroutine c = GameInput.instance.StartCoroutine(PlayWithDelay(delay));
        _runningCoroutines.Add(c);
        return;

        IEnumerator PlayWithDelay(float del)
        {
            yield return new WaitForSeconds(del);
            PlaySound(emitter);
        }
    }

    public void CancelAllDelayed()
    {
        foreach (Coroutine c in _runningCoroutines)
        {
            GameInput.instance.StopCoroutine(c);
        }

        _runningCoroutines.Clear();
    }

    private void PlaySound(FMOD_CustomEmitter emitter)
    {
        LastPlay = Time.time;

        if (_remainingSounds.Count == 0)
        {
            _remainingSounds.AddRange(_playedSounds);
            _playedSounds.Clear();
        }

        FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
        asset.path = _remainingSounds[0];
        emitter.SetAsset(asset);
        emitter.Play();

        _playedSounds.Add(_remainingSounds[0]);
        _remainingSounds.RemoveAt(0);
    }
}
