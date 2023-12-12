using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Creatures.Components;

partial class Carryable
{
    public bool isCarried;
    public Func<Carryable, CarryCreature, bool> CanAttach;
    public Action<Carryable, CarryCreature> Attached;
    public Action<Carryable, CarryCreature> Detached;

    private Creature creature;

    private float _nextCarryNoiseTime;
    private float _lastPickedUpTime;
    private List<Behaviour> _disabledComponents;
    private static readonly List<Type> toDisable =
    [
        typeof(WorldAmbientSoundPlayer),
        typeof(SwimBehaviour),
        typeof(Rigidbody),
        typeof(Creature)
    ];

    public void Awake()
    {
        _disabledComponents = [];
        creature = GetComponent<Creature>();
    }

    public bool CanBePickedUp(CarryCreature pickuper) => !isCarried && Time.time - _lastPickedUpTime > 5f
                && (CanAttach?.Multicast().All(f => f(this, pickuper)) ?? true);

    private void DisableComponents()
    {
        _disabledComponents.Clear();
        foreach (Type type in toDisable)
        {
            Behaviour comp = GetComponent(type) as Behaviour;
            if (!comp || !comp.enabled) continue;

            _disabledComponents.Add(comp);
            comp.enabled = false;
        }
    }

    private void RestoreComponents()
    {
        foreach (Behaviour component in _disabledComponents)
        {
            if (component) component.enabled = true;
        }
        _disabledComponents.Clear();
    }

    public void OnPickedUp(CarryCreature parent)
    {
        if (isCarried) RestoreComponents();
        DisableComponents();
        PlaySound(attachSounds);
        isCarried = true;
        _lastPickedUpTime = Time.time;
        _nextCarryNoiseTime = Time.time + carryNoiseInterval * (1 + Random.value);

        Attached?.Invoke(this, parent);
    }

    public void OnDropped(CarryCreature parent)
    {
        PlaySound(detachSounds);
        isCarried = false;
        RestoreComponents();

        Detached?.Invoke(this, parent);
    }

    public void FixedUpdate()
    {
        if (!isCarried) return;

        float time = Time.fixedTime;
        float deltaTime = Time.fixedDeltaTime;

        if (creature)
        {
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
        }

        if (time > _nextCarryNoiseTime)
        {
            _nextCarryNoiseTime = time + carryNoiseInterval * (1 + Random.value);
            PlaySound(carrySounds);
        }
    }

    private void PlaySound(string fmodEvent)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        emitter.PlayPath(fmodEvent);
    }
}
