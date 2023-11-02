using System;

namespace SCHIZO.Sounds.Collections;

partial class SoundCollectionInstance
{
    private FMODSoundCollection _fmodSounds;

    private void OnEnable()
    {
        _fmodSounds = FMODSoundCollection.For(collection, bus.GetBusName());
        path = id = new Guid().ToString();
    }

    public float LastPlay => _fmodSounds.LastPlay;

    public void Play2D(float delay = 0)
    {
        _fmodSounds.Play2D(delay);
    }

    public void Play(FMOD_CustomEmitter emitter, float delay = 0)
    {
        if (!emitter) throw new ArgumentNullException(nameof(emitter));
        _fmodSounds.Play(emitter, delay);
    }

    public void CancelAllDelayed() => _fmodSounds.CancelAllDelayed();

    public void Stop() => _fmodSounds.Stop();
}
