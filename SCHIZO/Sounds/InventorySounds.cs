using System;
using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Sounds;

public sealed class InventorySounds : MonoBehaviour
{
    [SerializeField] private Pickupable _pickupable;
    [SerializeField] private SoundPlayer _soundPlayer;

    private float _timer = -1;
    private Random _random;

    public static void Add(GameObject obj, SoundPlayer soundPlayer)
    {
        if (soundPlayer == null) throw new ArgumentNullException(nameof(soundPlayer));
        InventorySounds player = obj.AddComponent<InventorySounds>();
        player._soundPlayer = soundPlayer;
    }

    private void Awake()
    {
        if (_timer != -1) return;

        _random = new Random(GetInstanceID());
        _pickupable = GetComponent<Pickupable>();

        _timer = _random.Next(CONFIG.MinInventoryNoiseDelay, CONFIG.MaxInventoryNoiseDelay);
    }

    public void Update()
    {
        if (_timer == -1) Awake();

        if (CONFIG.DisableAllNoises) return;
        if (CONFIG.DisableInventoryNoises) return;

        if (!_pickupable || !Inventory.main.Contains(_pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = _random.Next(CONFIG.MinInventoryNoiseDelay, CONFIG.MaxInventoryNoiseDelay);
            // todo fix
            if (_soundPlayer == null)
                LOGGER.LogWarning($"no sound player on {name} {nameof(InventorySounds)}, cannot play");
            else
                _soundPlayer.Play2D();
        }
    }
}
