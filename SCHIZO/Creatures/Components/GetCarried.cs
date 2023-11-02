using System;
using System.Collections.Generic;
using SCHIZO.Sounds.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Creatures.Components;

partial class GetCarried
{
    public bool isCarried;

    private float nextCarryNoiseTime;
    private List<(MonoBehaviour component, bool wasEnabled)> _disabledComponents;
    private static readonly List<Type> toDisable = new()
    {
        typeof(WorldAmbientSoundPlayer),
        typeof(SwimBehaviour),
        typeof(Rigidbody),
    };

    public override void Awake()
    {
        base.Awake();
        // contrary to the name, this is actually the max possible priority
        // full explanation here <see cref="Events.Ermcon.ErmconAttendee.Awake"/>
        evaluatePriority = 99f;
        _disabledComponents = new List<(MonoBehaviour component, bool wasEnabled)>();
    }

    public override float Evaluate(float time) => isCarried ? 99f : -99f; // manual start/end

    private void DisableComponents()
    {
        _disabledComponents.Clear();
        foreach (Type type in toDisable)
        {
            MonoBehaviour comp = GetComponent(type) as MonoBehaviour;
            if (!comp) continue;

            _disabledComponents.Add((comp, comp.enabled));
            comp.enabled = false;
        }
    }

    private void RestoreComponents()
    {
        foreach ((MonoBehaviour component, bool wasEnabled) in _disabledComponents)
        {
            if (!component) continue;
            component.enabled = wasEnabled;
        }
    }

    public void OnPickedUp()
    {
        pickupSounds!?.Play(emitter);
        isCarried = true;
        StartPerform(Time.time);
    }

    public void OnDropped()
    {
        releaseSounds!?.Play(emitter);
        isCarried = false;
        StopPerform(Time.time);
    }

    public override void StartPerform(float time)
    {
        DisableComponents();
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
        if (likesBeingCarried)
        {
            creature.Happy.Add(deltaTime);
            creature.Friendliness.Add(deltaTime / 2f);
        }
        else
        {
            creature.Scared.Add(deltaTime);
            creature.Tired.Add(deltaTime / 2f);
        }

        if (time > nextCarryNoiseTime)
        {
            nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
            carrySounds!?.Play(emitter);
        }
    }

    public override void StopPerform(float time)
    {
        if (isCarried) OnDropped();
        RestoreComponents();
    }
}
