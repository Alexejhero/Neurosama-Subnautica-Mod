using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Utilities.Sounds;

public sealed class InventorySoundPlayer : MonoBehaviour
{
    private Pickupable _pickupable;
    private FMOD_CustomEmitter _emitter;
    private float _timer = -1;
    private Random _random;
    private SoundCollection _sounds;

    public static InventorySoundPlayer Add(GameObject obj, SoundCollection sounds)
    {
        InventorySoundPlayer player = obj.EnsureComponent<InventorySoundPlayer>();
        player._sounds = sounds;
        return player;
    }

    private void Awake()
    {
        if (_timer != -1) return;

        _random = new Random(GetInstanceID());

        _pickupable = GetComponent<Pickupable>();
        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;

        _timer = _random.Next(Plugin.CONFIG.MinInventoryNoiseDelay, Plugin.CONFIG.MaxInventoryNoiseDelay);
    }

    public void Update()
    {
        if (_timer == -1) Awake();

        if (Plugin.CONFIG.DisableAllNoises) return;
        if (Plugin.CONFIG.DisableInventoryNoises) return;

        if (!_pickupable || !Inventory.main.Contains(_pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = _random.Next(Plugin.CONFIG.MinInventoryNoiseDelay, Plugin.CONFIG.MaxInventoryNoiseDelay);
            _sounds.Play();
        }
    }
}
