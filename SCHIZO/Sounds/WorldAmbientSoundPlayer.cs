using JetBrains.Annotations;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Sounds;

partial class WorldAmbientSoundPlayer
{
    private FMODSoundCollection _fmodSoundCollection;
    private float _timer = -1;

    private void Awake()
    {
        _fmodSoundCollection = new FMODSoundCollection(soundCollection, ReflectionHelpers.GetFieldValue<string>(bus));
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
        _timer = Random.Range(SoundConfig.Provider.MinWorldSoundDelay, SoundConfig.Provider.MaxWorldSoundDelay);
    }

    [UsedImplicitly]
    private void OnKill()
    {
        enabled = false;
    }
}
