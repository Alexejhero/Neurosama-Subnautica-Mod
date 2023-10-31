using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Sounds.Players;

partial class InventoryAmbientSoundPlayer
{
    private float _timer = -1;

    private void Start()
    {
        ResetTimer();
    }

    public void Update()
    {
        if (!pickupable || !Inventory.main.Contains((Pickupable) pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            ResetTimer();
            Play();
        }
    }

    private void ResetTimer()
    {
        _timer = Random.Range(SoundConfig.Provider.MinInventorySoundDelay, SoundConfig.Provider.MaxInventorySoundDelay);
    }

    [UsedImplicitly]
    private void OnKill()
    {
        enabled = false;
    }
}
