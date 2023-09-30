using Nautilus.Utility;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Sounds;

namespace SCHIZO.Creatures.Tutel;

[RequireComponent(typeof(SwimBehaviour))]
public class GetCarried : RetargetCreatureAction
{
    public override void Awake()
    {
        base.Awake();
        // contrary to the name, this is actually the max possible priority
        evaluatePriority = 99f;
    }
    public override float Evaluate(float time) => isCarried ? 99f : -99f; // manual start/end

    public void OnPickedUp()
    {
        _pickupSounds.Play(emitter);
        isCarried = true;
        StartPerform(Time.time);
    }

    public void OnDropped()
    {
        _releaseSounds.Play(emitter);
        isCarried = false;
        StopPerform(Time.time);
    }

    public override void StartPerform(float time)
    {
        creature.GetComponent<SwimBehaviour>().Idle();
        creature.GetComponent<Rigidbody>().velocity = Vector3.zero;
        creature.GetComponent<WorldSounds>().enabled = false;
        nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
        if (!isCarried) OnPickedUp();
    }

    public override void Perform(float time, float deltaTime)
    {
        if (!isCarried)
        {
            StopPerform(time);
            return;
        }
        creature.Scared.Add(deltaTime);
        creature.Tired.Add(deltaTime / 2f);

        if (time > nextCarryNoiseTime)
        {
            nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
            _carrySounds.Play(emitter);
        }
    }

    public override void StopPerform(float time)
    {
        creature.GetComponent<WorldSounds>().enabled = true;
        if (isCarried) OnDropped();
    }

    public bool isCarried;
    public FMOD_CustomEmitter emitter;

    private static readonly SoundPlayer _pickupSounds = new(ResourceManager.LoadAsset<BaseSoundCollection>("Pickup by ermshark"), AudioUtils.BusPaths.UnderwaterCreatures);
    private static readonly SoundPlayer _carrySounds = new(ResourceManager.LoadAsset<BaseSoundCollection>("Carry by ermshark"), AudioUtils.BusPaths.UnderwaterCreatures);
    private static readonly SoundPlayer _releaseSounds = new(ResourceManager.LoadAsset<BaseSoundCollection>("Tutel Ambient"), AudioUtils.BusPaths.UnderwaterCreatures);

    private float nextCarryNoiseTime;
    private const float carryNoiseInterval = 5f;
}
