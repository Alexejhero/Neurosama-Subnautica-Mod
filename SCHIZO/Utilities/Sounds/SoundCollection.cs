using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SCHIZO.Utilities.Sounds;

public abstract class SoundCollection : ScriptableObject
{
    protected readonly List<string> _remainingSounds = new();
    protected readonly List<string> _playedSounds = new();

    private readonly List<Coroutine> _runningCoroutines = new();

    public virtual float LastPlay { get; protected set; } = -1;

    protected void Initialize(string path, string bus)
    {
        string dirpath = Path.Combine(AssetLoader.AssetsFolder, "sounds", path);

        foreach (string soundFile in Directory.GetFiles(dirpath))
        {
            string id = Guid.NewGuid().ToString();
            RegisterSound(id, soundFile, bus);
            _remainingSounds.Add(id);
        }

        _remainingSounds.Shuffle();
    }

    protected void StartSoundCoroutine(IEnumerator coroutine)
    {
        _runningCoroutines.Add(Player.main.StartCoroutine(coroutine));
    }

    protected abstract void RegisterSound(string id, string soundFile, string bus);

    public void CancelAllDelayed()
    {
        foreach (Coroutine c in _runningCoroutines)
        {
            GameInput.instance.StopCoroutine(c);
        }

        _runningCoroutines.Clear();
    }
}
