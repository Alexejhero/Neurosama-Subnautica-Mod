using JetBrains.Annotations;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Sounds;

partial class WorldAmbientSoundPlayer
{
    private FMODSoundCollection _fmodSoundCollection;
    private float _timer = -1;

    private void Awake()
    {
        _fmodSoundCollection = FMODSoundCollection.For(soundCollection, StaticHelpers.GetValue<string>(bus));
        ResetTimer();
    }

    private void Update()
    {
        if (pickupable && Inventory.main.Contains((Pickupable) pickupable)) return;
        if (constructable && !((Constructable) constructable).constructed) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            ResetTimer();
            _fmodSoundCollection.Play((FMOD_CustomEmitter) emitter);
        }
    }

    private void ResetTimer()
    {
        _timer = Random.Range(SoundConfig.Provider.MinWorldSoundDelay, SoundConfig.Provider.MaxWorldSoundDelay);
    }

    [UsedImplicitly]
    private void OnKill()
    {
        enabled = false;
    }
}
