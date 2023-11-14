using System;
using System.Collections.Generic;
using SCHIZO.Resources;
using SCHIZO.Sounds.Collections;
using SCHIZO.Sounds.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Creatures.Components;

partial class Carryable
{
    public bool isCarried;
    public Action<CarryCreature> PickedUp;
    public Action<CarryCreature> Dropped;

    private Creature creature;

    private float _nextCarryNoiseTime;
    private float _lastPickedUpTime;
    private List<Behaviour> _disabledComponents;
    private static readonly List<Type> toDisable = new()
    {
        typeof(WorldAmbientSoundPlayer),
        typeof(SwimBehaviour),
        typeof(Rigidbody),
        typeof(Creature),
    };

    public void Awake()
    {
        _disabledComponents = new List<Behaviour>();
        creature = GetComponent<Creature>();
    }

    public bool CanBePickedUp() => !isCarried && Time.time - _lastPickedUpTime > 5f;

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
        PickedUp?.Invoke(parent);
        PlaySound(attachSounds);
        isCarried = true;
        _lastPickedUpTime = Time.time;
        _nextCarryNoiseTime = Time.time + carryNoiseInterval * (1 + Random.value);
    }

    public void OnDropped(CarryCreature parent)
    {
        PlaySound(detachSounds);
        Dropped?.Invoke(parent);
        isCarried = false;
        RestoreComponents();
    }

    public void FixedUpdate()
    {
        if (!isCarried)
        {
            OnDropped(null);
            return;
        }

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

    private void PlaySound(SoundCollectionInstance coll)
    {
        if (Assets.Mod_Options_DisableAllSounds.Value) return;
        coll!?.PlayRandom3D(emitter);
    }
}
