using System;
using System.Collections.Generic;
using Nautilus.Utility;
using SCHIZO.Sounds.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Creatures.Components;

partial class GetCarried
{
    public bool isCarried;

    private float _nextCarryNoiseTime;
    private float _lastPickedUpTime;
    private List<(MonoBehaviour component, bool wasEnabled)> _disabledComponents;
    private static readonly List<Type> toDisable = new()
    {
        typeof(WorldAmbientSoundPlayer),
        typeof(SwimBehaviour),
        typeof(Rigidbody),
    };

    private const string BUS = AudioUtils.BusPaths.UnderwaterCreatures;
    public override void Awake()
    {
        base.Awake();
        // contrary to the name, this is actually the max possible priority
        // full explanation here <see cref="Events.Ermcon.ErmconAttendee.Awake"/>
        evaluatePriority = 99f;
        pickupSounds = pickupSounds!?.Initialize(BUS);
        carrySounds = carrySounds!?.Initialize(BUS);
        releaseSounds = releaseSounds!?.Initialize(BUS);
        _disabledComponents = new List<(MonoBehaviour component, bool wasEnabled)>();
    }
    public override float Evaluate(float time) => isCarried ? 99f : -99f; // manual start/end

    public bool CanBePickedUp() => Time.time - _lastPickedUpTime > 5f;

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
        _lastPickedUpTime = Time.time;
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
        _nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
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

        if (time > _nextCarryNoiseTime)
        {
            _nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
            carrySounds!?.Play(emitter);
        }
    }

    public override void StopPerform(float time)
    {
        if (isCarried) OnDropped();
        RestoreComponents();
    }
}
