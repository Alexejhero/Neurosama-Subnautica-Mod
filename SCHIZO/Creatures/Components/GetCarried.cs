using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Components;

partial class GetCarried : CustomCreatureAction
{
    public bool isCarried;
    public FMOD_CustomEmitter emitter;

    private float nextCarryNoiseTime;
    private FMODSoundCollection _pickupSounds;
    private FMODSoundCollection _carrySounds;
    private FMODSoundCollection _releaseSounds;

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
        WorldAmbientSoundPlayer worldSounds = creature.GetComponent<WorldAmbientSoundPlayer>();
        if (worldSounds) worldSounds.enabled = false;
        StartPerform(Time.time);
    }

    public void OnDropped()
    {
        _releaseSounds.Play(emitter);
        isCarried = false;
        WorldAmbientSoundPlayer worldSounds = creature.GetComponent<WorldAmbientSoundPlayer>();
        if (worldSounds) worldSounds.enabled = true;
        StopPerform(Time.time);
    }

    public override void StartPerform(float time)
    {
        creature.GetComponent<SwimBehaviour>().Idle();
        creature.GetComponent<Rigidbody>().velocity = Vector3.zero;
        creature.GetComponent<WorldAmbientSoundPlayer>().enabled = false;
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
        if (isCarried) OnDropped();
    }
}
