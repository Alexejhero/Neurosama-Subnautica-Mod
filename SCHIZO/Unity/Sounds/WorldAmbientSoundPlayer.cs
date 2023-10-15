using UnityEngine;

namespace SCHIZO.Unity.Sounds;

partial class WorldAmbientSoundPlayer
{
    private SCHIZO.Sounds.FMODSoundCollection _fmodSoundCollection;
    private float _timer = -1;

    private void Awake()
    {
        _fmodSoundCollection = new SCHIZO.Sounds.FMODSoundCollection(soundCollection, SCHIZO.Sounds.FMODSoundCollection.GetBusPath(bus));
        ResetTimer();
    }

    private void Update()
    {
        if (pickupable && Inventory.main.Contains((Pickupable) pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            ResetTimer();
            _fmodSoundCollection.Play((FMOD_CustomEmitter) emitter);
        }
    }

    private void ResetTimer()
    {
        _timer = Random.Range(SCHIZO.Sounds.SoundConfig.Provider.MinWorldSoundDelay, SCHIZO.Sounds.SoundConfig.Provider.MaxWorldSoundDelay);
    }

    private void OnKill()
    {
        enabled = false;
    }
}
