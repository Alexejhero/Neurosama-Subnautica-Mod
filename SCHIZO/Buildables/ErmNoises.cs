using UnityEngine;
using Random = System.Random;

namespace SCHIZO.Buildables;

public sealed class ErmNoises : MonoBehaviour
{
    private FMOD_CustomEmitter _emitter;
    private float _timer = -1;
    private Random _random;

    private void Awake()
    {
        if (_timer != -1) return;

        _random = new Random(GetInstanceID());

        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;

        _timer = _random.Next(Plugin.Config.MinWorldNoiseDelay, Plugin.Config.MaxWorldNoiseDelay);
    }

    public void Update()
    {
        if (_timer == -1) Awake();

        if (Plugin.Config.DisableAllNoises) return;
        if (Plugin.Config.DisableWorldNoises) return;

        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = _random.Next(Plugin.Config.MinWorldNoiseDelay, Plugin.Config.MaxWorldNoiseDelay);
            BuildablesLoader.ErmWorldSounds.Play(_emitter);
        }
    }
}
