using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Sounds;

partial class WorldAmbientSoundPlayer
{
    private float _timer = -1;

    private void Start()
    {
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
            Play();
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
