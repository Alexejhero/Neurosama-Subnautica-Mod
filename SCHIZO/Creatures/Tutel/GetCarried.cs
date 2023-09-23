using SCHIZO.Sounds;
using UnityEngine;

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
        pickupSounds.Play(emitter);
        isCarried = true;
        StartPerform(Time.time);
    }

    public void OnDropped()
    {
        releaseSounds.Play(emitter);
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
            carrySounds.Play(emitter);
        }
    }

    public override void StopPerform(float time)
    {
        creature.GetComponent<WorldSounds>().enabled = true;
        if (isCarried) OnDropped();
    }

    public bool isCarried;
    public FMOD_CustomEmitter emitter;

    private static readonly SoundCollection pickupSounds = SoundCollection.Combine(TutelLoader.Sounds.PickupSounds, TutelLoader.Sounds.HurtSounds);
    private static readonly SoundCollection releaseSounds = SoundCollection.Combine(TutelLoader.WorldSounds);
    private static readonly SoundCollection carrySounds = SoundCollection.Combine(TutelLoader.Sounds.CraftSounds, TutelLoader.Sounds.EatSounds);
    private float nextCarryNoiseTime;
    private const float carryNoiseInterval = 5f;
}
