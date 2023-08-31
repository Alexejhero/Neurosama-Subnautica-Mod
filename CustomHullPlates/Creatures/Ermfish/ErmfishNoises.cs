using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Creatures.Ermfish;

public sealed class ErmfishNoises : MonoBehaviour
{
    private Pickupable _pickupable;
    private FMOD_CustomEmitter _emitter;
    private float _inventoryTimer = -1;
    private float _worldTimer = -1;
    private Random _random;

    private void Awake()
    {
        if (_inventoryTimer != -1) return;

        _random = new Random(GetInstanceID());

        _pickupable = GetComponent<Pickupable>();
        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;

        _inventoryTimer = _random.Next(Plugin.Config.MinInventoryNoiseDelay, Plugin.Config.MaxInventoryNoiseDelay);
        _worldTimer = _random.Next(Plugin.Config.MinWorldNoiseDelay, Plugin.Config.MaxWorldNoiseDelay);
    }

    public void Update()
    {
        if (_inventoryTimer == -1) Awake();

        if (Plugin.Config.DisableAllNoises) return;

        if (!_pickupable || !Inventory.main.Contains(_pickupable)) WorldUpdate();
        else InventoryUpdate();
    }

    private void InventoryUpdate()
    {
        if (Plugin.Config.DisableInventoryNoises) return;

        _inventoryTimer -= Time.deltaTime;

        if (_inventoryTimer < 0)
        {
            _inventoryTimer = _random.Next(Plugin.Config.MinInventoryNoiseDelay, Plugin.Config.MaxInventoryNoiseDelay);
            ErmfishLoader.InventorySounds.Play();
        }
    }

    private void WorldUpdate()
    {
        if (Plugin.Config.DisableWorldNoises) return;

        _worldTimer -= Time.deltaTime;

        if (_worldTimer < 0)
        {
            _worldTimer = _random.Next(Plugin.Config.MinWorldNoiseDelay, Plugin.Config.MaxWorldNoiseDelay);
            ErmfishLoader.WorldSounds.Play(_emitter);
        }
    }
}
