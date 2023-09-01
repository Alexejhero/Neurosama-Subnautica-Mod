using System;
using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Sounds;

public sealed class InventorySoundPlayer : MonoBehaviour
{
    [SerializeField] private Pickupable _pickupable;
    [SerializeField] private FMOD_CustomEmitter _emitter;
    [SerializeField] private SoundCollection2D _sounds;

    private float _timer = -1;
    private Random _random;

    public static void Add(GameObject obj, SoundCollection2D sounds)
    {
        if (sounds == null) throw new ArgumentNullException(nameof(sounds));
        InventorySoundPlayer player = obj.AddComponent<InventorySoundPlayer>();
        player._sounds = sounds;
    }

    private void Awake()
    {
        if (_timer != -1) return;

        _random = new Random(GetInstanceID());

        _pickupable = GetComponent<Pickupable>();
        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;

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
            _sounds.Play();
        }
    }
}
