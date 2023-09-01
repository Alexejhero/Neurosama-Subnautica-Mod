using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Utilities.Sounds;

public sealed class WorldSoundPlayer : MonoBehaviour
{
    private Pickupable _pickupable;
    private FMOD_CustomEmitter _emitter;
    private SoundCollection3D _sounds;
    private float _timer = -1;
    private Random _random;

    public static WorldSoundPlayer Add(GameObject obj, SoundCollection3D sounds)
    {
        WorldSoundPlayer player = obj.EnsureComponent<WorldSoundPlayer>();
        player._sounds = sounds;
        return player;
    }

    private void Awake()
    {
        _random = new Random(GetInstanceID());

        _pickupable = GetComponent<Pickupable>();
        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;

        _timer = _random.Next(Plugin.CONFIG.MinWorldNoiseDelay, Plugin.CONFIG.MaxWorldNoiseDelay);
    }

    private void Update()
    {
        if (Plugin.CONFIG.DisableAllNoises) return;
        if (Plugin.CONFIG.DisableWorldNoises) return;

        if (_pickupable && Inventory.main.Contains(_pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = _random.Next(Plugin.CONFIG.MinWorldNoiseDelay, Plugin.CONFIG.MaxWorldNoiseDelay);
            _sounds.Play(_emitter);
        }
    }
}
