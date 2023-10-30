using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SCHIZO.Sounds;

partial class SoundCollection
{
    private readonly Dictionary<string, SoundCollectionInstance> _instances = new();

    [MustUseReturnValue]
    public virtual SoundCollectionInstance Initialize(string bus)
    {
        if (_instances.TryGetValue(bus, out SoundCollectionInstance instance)) return instance;
        return _instances[bus] = SoundCollectionInstance.Create(this, bus);
    }

    public virtual float LastPlay => throw new InvalidOperationException($"SoundCollection {this} has not been initialized");
    public virtual void Play2D(float delay = 0) => throw new InvalidOperationException($"SoundCollection {this} has not been initialized");
    public virtual void Play(FMOD_CustomEmitter emitter, float delay = 0) => throw new InvalidOperationException($"SoundCollection {this} has not been initialized");
    public virtual void CancelAllDelayed() => throw new InvalidOperationException($"SoundCollection {this} has not been initialized");
}
