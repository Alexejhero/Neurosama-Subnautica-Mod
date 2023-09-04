using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SCHIZO.DataStructures;
using UnityEngine;

namespace SCHIZO.Sounds;

public abstract class SoundCollection : ScriptableObject
{
    protected readonly RandomList<string> _sounds = new();

    private readonly List<Coroutine> _runningCoroutines = new();

    public virtual float LastPlay { get; protected set; } = -1;

    protected void Initialize(string path, string bus)
    {
        string dirpath = Path.Combine(AssetLoader.AssetsFolder, "sounds", path);

        MD5 md5 = MD5.Create();
        foreach (string soundFile in Directory.GetFiles(dirpath))
        {
            byte[] pathHash = md5.ComputeHash(Encoding.UTF8.GetBytes(Path.GetFileName(soundFile)));
            string id = new Guid(pathHash).ToString();
            RegisterSound(id, soundFile, bus);
            _sounds.Add(id);
        }

        _sounds.Shuffle();
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
