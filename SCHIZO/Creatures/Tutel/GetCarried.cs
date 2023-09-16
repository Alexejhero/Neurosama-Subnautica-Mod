using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[RequireComponent(typeof(SwimBehaviour))]
public class GetCarried : CreatureAction
{
    private new void Awake()
    {
        // contrary to the name, this is actually the max possible priority
        evaluatePriority = 99f;
    }
    public override float Evaluate(Creature creat, float time) => isCarried ? 99f : -99f; // manual start/end

    public void OnPickedUp()
    {
        pickupSounds.Play(emitter);
        isCarried = true;
        StartPerform(GetComponent<Creature>(), Time.time);
    }

    public void OnDropped()
    {
        releaseSounds.Play(emitter);
        isCarried = false;
        StopPerform(gameObject.GetComponent<Creature>(), Time.time);
    }

    public override void StartPerform(Creature creat, float time)
    {
        creat.GetComponent<SwimBehaviour>().Idle();
        creat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        creat.GetComponent<WorldSoundPlayer>().enabled = false;
        nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
        if (!isCarried) OnPickedUp();
    }

    public override void Perform(Creature creat, float time, float deltaTime)
    {
        if (!isCarried)
        {
            StopPerform(creat, time);
            return;
        }
        creat.Scared.Add(deltaTime);
        creat.Tired.Add(deltaTime/2f);

        if (time > nextCarryNoiseTime)
        {
            nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
            carrySounds.Play(emitter);
        }
    }

    public override void StopPerform(Creature creat, float time)
    {
        creat.GetComponent<WorldSoundPlayer>().enabled = true;
        if (isCarried) OnDropped();
    }

    public bool isCarried;
    public FMOD_CustomEmitter emitter;

    private static readonly SoundCollection pickupSounds = SoundCollection.Combine(TutelLoader.Sounds.PickupSounds, TutelLoader.Sounds.ScanSounds, TutelLoader.Sounds.HurtSounds);
    private static readonly SoundCollection releaseSounds = SoundCollection.Combine(TutelLoader.WorldSounds);
    private static readonly SoundCollection carrySounds = SoundCollection.Combine(TutelLoader.Sounds.CraftSounds, TutelLoader.Sounds.EatSounds);
    private float nextCarryNoiseTime;
    private float carryNoiseInterval = 5f;
}
