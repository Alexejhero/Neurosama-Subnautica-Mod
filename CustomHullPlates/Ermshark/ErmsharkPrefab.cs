using System.Collections;
using System.Collections.Generic;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using UnityEngine;

namespace SCHIZO.Ermshark;

public sealed class ErmsharkPrefab : CreatureAsset
{
    public ErmsharkPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        const float swimVelocity = 2;// 10f;

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
        GameObject model = new("Ermshark model");
        model.SetActive(false);

        GameObject worldModel = new("Model");

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = worldModel.transform;
        cube.transform.localScale = Vector3.one * 0.85f;
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = worldModel.transform;
        sphere.transform.localPosition = Vector3.forward * 0.5f;

        worldModel.transform.parent = model.transform;
        worldModel.AddComponent<Animator>();
        worldModel.transform.localScale = Vector3.one * 2;

        foreach (Collider col in model.GetComponentsInChildren<Collider>(true))
        {
            GameObject.DestroyImmediate(col);
        }

        model.gameObject.AddComponent<BoxCollider>().size = Vector3.one * 2;

        GameObject tailRoot = new("Tail");
        tailRoot.transform.parent = worldModel.transform;
        tailRoot.transform.localScale = Vector3.one;

        Transform parent = tailRoot.transform;

        for (int i = 0; i < 4; i++)
        {
            var tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tail.name = "TailSegment_phys";
            GameObject.DestroyImmediate(tail.GetComponent<Collider>());
            tail.transform.parent = parent;
            tail.transform.localPosition = Vector3.forward * (-0.4f * (Mathf.Log(i + 2)));
            tail.transform.localScale = Vector3.one * 0.87f;
            parent = tail.transform;
        }

        GameObject mouth = new("Mouth");
        mouth.transform.parent = sphere.transform;
        mouth.transform.ZeroTransform();
        mouth.AddComponent<SphereCollider>().isTrigger = true;

        GameObject.DontDestroyOnLoad(model);

        return model;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        TrailManagerBuilder trailManagerBuilder = new(components, prefab.transform.SearchChild("Tail"));
        trailManagerBuilder.SegmentSnapSpeed = 4;
        trailManagerBuilder.SetTrailArrayToPhysBoneChildren();
        trailManagerBuilder.AllowDisableOnScreen = false;
        trailManagerBuilder.Apply();

        GameObject mouth = prefab.SearchChild("Mouth");
        CreaturePrefabUtils.AddMeleeAttack<LeviathanMeleeAttack>(prefab, components, mouth, true, 40f);

        ErmsharkData.Prefab = prefab;

        yield break;
    }
}

internal class LeviathanMeleeAttack : MeleeAttack
{
    public override void OnTouch(Collider collider)
    {
        base.OnTouch(collider);
        ErrorMessage.AddMessage("Chomp!");
    }
}
