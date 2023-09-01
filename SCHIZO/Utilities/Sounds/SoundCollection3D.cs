using System.Collections;
using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Utilities.Sounds;

public sealed class SoundCollection3D : SoundCollection
{
    private SoundCollection3D() {}

    public static SoundCollection3D Create(string path, string bus)
    {
        SoundCollection3D result = CreateInstance<SoundCollection3D>();
        result.Initialize(path, bus);
        return result;
    }

    protected override void RegisterSound(string id, string soundFile, string bus)
    {
        Sound s = CustomSoundHandler.RegisterCustomSound(id, soundFile, bus, MODE._3D | MODE._3D_LINEARSQUAREROLLOFF);
        s.set3DMinMaxDistance(1, 30);
    }

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        if (!emitter) return;
        if (CONFIG.DisableAllNoises) return;

        if (delay == 0)
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

    private void PlaySound(FMOD_CustomEmitter emitter)
    {
        LastPlay = Time.time;

        if (_remainingSounds.Count == 0)
        {
            _remainingSounds.AddRange(_playedSounds);
            _playedSounds.Clear();
        }

        FMODAsset asset = CreateInstance<FMODAsset>();
        asset.path = _remainingSounds[0];
        emitter.SetAsset(asset);
        emitter.Play();

        _playedSounds.Add(_remainingSounds[0]);
        _remainingSounds.RemoveAt(0);
    }
}
