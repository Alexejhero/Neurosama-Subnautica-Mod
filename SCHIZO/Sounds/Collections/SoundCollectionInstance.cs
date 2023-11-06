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

    public void PlayRandom2D(float delay = 0) => _fmodSounds.PlayRandom2D(delay);

    public void Play2D(int index, float delay = 0) => _fmodSounds.Play2D(index, delay);

    public void PlayRandom3D(FMOD_CustomEmitter emitter, float delay = 0) => _fmodSounds.PlayRandom3D(emitter, delay);

    public void Play3D(int index, FMOD_CustomEmitter emitter, float delay = 0) => _fmodSounds.Play3D(index, emitter, delay);

    public void CancelAllDelayed() => _fmodSounds.CancelAllDelayed();

    public void Stop() => _fmodSounds.Stop();
}
