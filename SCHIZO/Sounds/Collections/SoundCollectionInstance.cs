using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Sounds.Collections;

public sealed class SoundCollectionInstance : SoundCollection
{
    private FMODSoundCollection _fmodSounds;
    private string _bus;

    public static SoundCollectionInstance Create(SoundCollection soundCollection, string bus)
    {
        SoundCollectionInstance instance = CreateInstance<SoundCollectionInstance>();
        instance._fmodSounds = FMODSoundCollection.For(soundCollection, bus);
        instance._bus = bus;
        return instance;
    }

    [Obsolete("SoundCollectionInstance does not need to be initialized", true)]
    public override SoundCollectionInstance Initialize(string bus)
    {
        if (_bus != bus) throw new InvalidOperationException($"SoundCollection {this} is already initialized with a different bus ({_bus} != {bus})");
        return this;
    }

    public override float LastPlay => _fmodSounds.LastPlay;
    public override void Play2D(float delay = 0) => _fmodSounds.Play2D(delay);
    public override void Play(FMOD_CustomEmitter emitter, float delay = 0) => _fmodSounds.Play(emitter, delay);
    public override void CancelAllDelayed() => _fmodSounds.CancelAllDelayed();

    [Obsolete("SoundCollectionInstance does not support enumerating sounds", true)]
    public override IEnumerable<AudioClip> GetSounds() => throw new InvalidOperationException("SoundCollectionInstance does not support enumerating sounds");
}
