using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Utility;
using SCHIZO.Extensions;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CreatureAsset
{
    public ErmsharkPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    private static GameObject Prefab => ResourceManager.LoadAsset<GameObject>("erm_shark");

    public const float swimVelocity = 8f;
    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new(Prefab, BehaviourType.Shark, EcoTargetType.Shark, 20)
        {
            CellLevel = LargeWorldEntity.CellLevel.Medium,
            SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(30, 5, 30), 2, 1),
            StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 60),
            AvoidObstaclesData = new AvoidObstaclesData(0.6f, swimVelocity, true, 5, 5),
            AcidImmune = true,
            Mass = 120,
            EyeFOV = -0.9f,
            LocomotionData = new LocomotionData(10, 0.45f),
            AnimateByVelocityData = new AnimateByVelocityData(swimVelocity * 1.2f),
            AttackLastTargetData = new AttackLastTargetData(1, swimVelocity * 1.25f, 0.5f, 5f),
            AttackCyclopsData = new AttackCyclopsData(1f, 15f, 100f, 0.4f, 4.5f, 0.08f),
            BehaviourLODData = new BehaviourLODData(50, 100, 150),
            RespawnData = new RespawnData(false),
        };
        template.WithoutInfection();
        template.SetCreatureComponentType<ErmsharkBehaviour>();

        return template;
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

        ErmsharkLoader.Prefab = prefab;

        prefab.EnsureComponentFields();

        yield break;
    }

    public override void ApplyMaterials(GameObject prefab) => MaterialUtils.ApplySNShaders(prefab, 1f);
}
