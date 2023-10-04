using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using SCHIZO.API;
using SCHIZO.API.Creatures;
using SCHIZO.API.Extensions;
using SCHIZO.API.Sounds;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CustomCreaturePrefab<ErmsharkBehaviour>
{
    public const float swimVelocity = 8f;

    public static GameObject Prefab;

    public ErmsharkPrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem, creaturePrefab)
    {
        BehaviourType = BehaviourType.Shark;
        EcoTargetType = EcoTargetType.Shark;
        MaxHealth = 20;

        SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(30, 5, 30), 2, 1);
        StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 60);
        AvoidObstaclesData = new AvoidObstaclesData(0.6f, swimVelocity, true, 5, 5);
        Mass = 120;
        EyeFOV = -0.9f;
        LocomotionData = new LocomotionData(10, 0.45f);
        AnimateByVelocityData = new AnimateByVelocityData(swimVelocity * 1.2f);
        AttackLastTargetData = new AttackLastTargetData(1, swimVelocity * 1.25f, 0.5f, 5f);
        AttackCyclopsData = new AttackCyclopsData(1f, 15f, 100f, 0.4f, 4.5f, 0.08f);
        BehaviourLODData = new BehaviourLODData(50, 100, 150);
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        WorldSounds.Add(prefab, CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermshark).AmbientWorldSounds);

        BullyTutel bully = prefab.AddComponent<BullyTutel>();
        bully.mouth = bully.tutelAttach = prefab.SearchChild("mouth_attach_point").transform;

        AggressiveWhenSeePlayer aggressive = prefab.AddComponent<AggressiveWhenSeePlayer>();
        aggressive.maxRangeMultiplier = CreaturePrefabUtils.maxRangeMultiplierCurve;
        aggressive.distanceAggressionMultiplier = CreaturePrefabUtils.distanceAggressionMultiplierCurve;
        aggressive.lastTarget = components.LastTarget;
        aggressive.creature = components.Creature;
        aggressive.targetType = EcoTargetType.Shark;
        aggressive.aggressionPerSecond = 2;
        aggressive.maxRangeScalar = 75;
        aggressive.maxSearchRings = 3;
        aggressive.ignoreSameKind = true;

        GameObject mouth = prefab.SearchChild("attack_collider");
        CreaturePrefabUtils.AddMeleeAttack<ErmsharkAttack>(prefab, components, mouth, true, 20);

        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Acid, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Cold, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Fire, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Poison, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Radiation, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Starve, 0f);

        Prefab = prefab;

        prefab.EnsureComponentFields();

        yield break;
    }
}
