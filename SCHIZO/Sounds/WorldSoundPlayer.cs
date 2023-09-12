using System;
using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Sounds;

public sealed class WorldSoundPlayer : MonoBehaviour
{
    public FMOD_CustomEmitter emitter;
    [SerializeField] private Pickupable _pickupable;
    [SerializeField] private SoundCollection _sounds;

    private float _timer = -1;
    private Random _random;

    public static void Add(GameObject obj, SoundCollection sounds)
    {
        if (sounds == null) throw new ArgumentNullException(nameof(sounds));
        WorldSoundPlayer player = obj.AddComponent<WorldSoundPlayer>();
        player._sounds = sounds;
    }

    private void Awake()
    {
        _random = new Random(GetInstanceID());

        _pickupable = GetComponent<Pickupable>();
        emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        emitter.followParent = true;

        _timer = _random.Next(CONFIG.MinWorldNoiseDelay, CONFIG.MaxWorldNoiseDelay);
    }

    private void Update()
    {
        if (CONFIG.DisableAllNoises) return;
        if (CONFIG.DisableWorldNoises) return;

        if (_pickupable && Inventory.main.Contains(_pickupable)) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = _random.Next(CONFIG.MinWorldNoiseDelay, CONFIG.MaxWorldNoiseDelay);
            _sounds.Play(emitter);
        }
    }
}
