using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Sounds.Players;

partial class InventoryAmbientSoundPlayer
{
    private float _timer = -1;

    private void Awake()
    {
        ResetTimer();
    }

    private void OnDestroy()
    {
        Stop();
    }

    public void Update()
    {
        if (!pickupable || !Inventory.main.Contains((Pickupable) pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            ResetTimer();
            if (!disabledOption || !disabledOption.Value)
                Play();
        }
    }

    private void ResetTimer()
    {
        _timer = Random.Range(MinDelay.GetValue(), MaxDelay.GetValue());
    }

    [UsedImplicitly]
    private void OnKill()
    {
        enabled = false;
    }
}
