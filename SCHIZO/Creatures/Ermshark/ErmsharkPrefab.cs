using System.Collections;
using System.Collections.Generic;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Utility;
using SCHIZO.Utilities;
using SCHIZO.Utilities.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CreatureAsset
{
    public ErmsharkPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    private static GameObject Prefab => AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>("erm_shark");

    public override CreatureTemplate CreateTemplate()
    {
        const float swimVelocity = 8f;

        CreatureTemplate template = new(Prefab, BehaviourType.Shark, EcoTargetType.Shark, 20) // TODO: Figure out damage modifiers
        {
            CellLevel = LargeWorldEntity.CellLevel.Medium,
            SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(30, 5, 30), 2, 1),
            StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 60),
            AvoidObstaclesData = new AvoidObstaclesData(0.6f, swimVelocity, true, 5, 5),
            AcidImmune = true,
            Mass = 120, // TODO: figure out mass
            EyeFOV = -0.9f,
            LocomotionData = new LocomotionData(10, 0.45f),
            AnimateByVelocityData = new AnimateByVelocityData(swimVelocity * 1.2f),
            AttackLastTargetData = new AttackLastTargetData(1, swimVelocity * 1.25f, 0.5f, 5f),
            AttackCyclopsData = new AttackCyclopsData(1f, 15f, 100f, 0.4f, 4.5f, 0.08f),
            AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>
            {
                new(EcoTargetType.Shark, 2, 75, 3),
            },
            BehaviourLODData = new BehaviourLODData(50, 100, 150),
            CanBeInfected = false,
        };
        template.SetCreatureComponentType<Ermshark>();

        return template;
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        WorldSoundPlayer.Add(prefab, ErmsharkLoader.AmbientSounds);

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
        aggressive.targetShouldBeInfected = false;
        aggressive.minimumVelocity = 0;
        aggressive.hungerThreshold = 0;

        GameObject mouth = prefab.SearchChild("attack_collider");
        CreaturePrefabUtils.AddMeleeAttack<MeleeAttack>(prefab, components, mouth, true, 20);

        ErmsharkData.Prefab = prefab;

        yield break;
    }

    public override void ApplyMaterials(GameObject prefab) => MaterialUtils.ApplySNShaders(prefab, 1f);
}
