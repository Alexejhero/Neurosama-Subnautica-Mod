using System.Collections;
using System.Collections.Generic;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CreatureAsset
{
    public ErmsharkPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    private static GameObject Prefab => AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>("erm_shark");

    protected override CreatureTemplate CreateTemplate()
    {
        const float swimVelocity = 2;// TODO: 10f;

        CreatureTemplate template = new(GetModel(), BehaviourType.Shark, EcoTargetType.Shark, 20) // TODO: Figure out health
        {
            CellLevel = LargeWorldEntity.CellLevel.Medium,
            SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(30, 10, 30), 2, 1),
            StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 60),
            AvoidTerrainData = new AvoidTerrainData(0.7f, swimVelocity, 30, 30),
            AcidImmune = true,
            BioReactorCharge = 1200,
            Mass = 120, // TODO: figure out mass
            EyeFOV = -0.6f,
            LocomotionData = new LocomotionData(10, 0.45f),
            SizeDistribution = new AnimationCurve(new Keyframe(0, 0.75f), new Keyframe(1, 1f)),
            AnimateByVelocityData = new AnimateByVelocityData(swimVelocity),
            AttackLastTargetData = new AttackLastTargetData(0.8f, swimVelocity * 1.25f, 0.5f, 5f),
            AttackCyclopsData = new AttackCyclopsData(1f, 15f, 100f, 0.4f, 4.5f, 0.08f),
            AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
            {
                new(EcoTargetType.Shark, 2, 75, 3),
            },
            BehaviourLODData = new BehaviourLODData(50, 100, 150),
            CanBeInfected = false,
        };
        template.SetCreatureComponentType<Ermshark>();

        return template;
    }

    private static GameObject GetModel()
    {
        GameObject shark = GameObject.Instantiate(Prefab);
        shark.SetActive(false);

        GameObject.DontDestroyOnLoad(shark);

        return shark;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        GameObject mouth = prefab.SearchChild("attack_collider");
        CreaturePrefabUtils.AddMeleeAttack<MeleeAttack>(prefab, components, mouth, true, 15);

        ErmsharkData.Prefab = prefab;

        yield break;
    }
}
