using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using Nautilus.Assets;
using Nautilus.Utility;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

public class TutelPrefab : CreatureAsset
{
	public TutelPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
	{
	}

	private static GameObject Prefab => AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>("tutel_creature");

	private const float swimVelocity = 2f;
	public override CreatureTemplate CreateTemplate()
	{
		CreatureTemplate template = new(Prefab, BehaviourType.Crab, EcoTargetType.Coral, float.MaxValue)
		{
            CreatureComponentType = typeof(CaveCrawler),
            // Cell level needs to be set to Near to avoid our spawns falling through the unrendered map which is too far
            // fish are not affected by this, but walking creatures are, as they have gravity on
			CellLevel = LargeWorldEntity.CellLevel.Near,
			Mass = 20,
			BioReactorCharge = 0,
			EyeFOV = 0,
			ScareableData = new ScareableData(),
			PickupableFishData = new PickupableFishData(TechType.Floater, "WM", "VM"),
			EdibleData = new EdibleData(13, -7, false, 1f),
			ScannerRoomScannable = true,
			CanBeInfected = false,
			SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
			AnimateByVelocityData = new AnimateByVelocityData(swimVelocity),
            // Walk on surface and swim above water true
            LocomotionData = new LocomotionData(10f, 2f, 1f, 0.1f, true, true, true),
		};
		template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));

		return template;
	}

	public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
	{
        prefab.GetComponent<HeldFish>().ikAimLeftArm = true;

        // ECC forces SwimBehaviour but we want WalkBehaviour
        // these depend on SwimBehaviour so we have to destroy them first
        Object.DestroyImmediate(prefab.GetComponent<FleeOnDamage>());
        Object.DestroyImmediate(prefab.GetComponent<SwimRandom>());
        Object.DestroyImmediate(prefab.GetComponent<SwimBehaviour>());

        // for hurt sounds (otherwise GetComponent in patches picks the looping emitter)
        prefab.AddComponent<FMOD_CustomEmitter>();

        CaveCrawler crawler = prefab.GetComponent<CaveCrawler>();
        crawler.rb = prefab.GetComponentInChildren<Rigidbody>();
        crawler.walkingSound = prefab.EnsureComponent<FMOD_CustomLoopingEmitter>(); // empty
        crawler.jumpSound = AudioUtils.GetFmodAsset("event:/sub/common/fishsplat"); // placeholder
        crawler.aliveCollider = prefab.GetComponentInChildren<Collider>();

        WalkBehaviour walk = prefab.EnsureComponent<WalkBehaviour>();
        walk.onSurfaceMovement = prefab.EnsureComponent<OnSurfaceMovement>();
        walk.splineFollowing = prefab.GetComponent<SplineFollowing>();
        walk.onSurfaceMovement.locomotion = prefab.GetComponent<Locomotion>();

        CaveCrawlerGravity gravity = prefab.EnsureComponent<CaveCrawlerGravity>();
        gravity.crawlerRigidbody = crawler.rb;
        gravity.caveCrawler = crawler;
        gravity.liveMixin = crawler.liveMixin;

        // if the OnSurfaceTracker is added earlier, the tutel slides around everywhere
        // (see OnSurfaceTracker's and CaveCrawlerGravity's FixedUpdate)
        crawler.onSurfaceTracker = prefab.EnsureComponent<OnSurfaceTracker>();
        walk.onSurfaceTracker = crawler.onSurfaceTracker;
        walk.onSurfaceMovement.onSurfaceTracker = crawler.onSurfaceTracker;

        MoveOnSurface moveSurface = prefab.AddComponent<MoveOnSurface>();
        moveSurface.creature = crawler;
        moveSurface.walkBehaviour = walk;
        moveSurface.onSurfaceTracker = walk.onSurfaceTracker;
        moveSurface.moveVelocity = swimVelocity;

        FleeOnDamage fleeDamage = prefab.EnsureComponent<FleeOnDamage>();
        fleeDamage.creature = crawler;
        fleeDamage.damageThreshold = 0.01f; // very easily scared tutel
        fleeDamage.swimVelocity = swimVelocity * 1.5f;
        FleeWhenScared fleeScared = prefab.EnsureComponent<FleeWhenScared>();
        fleeScared.creature = crawler;
        fleeScared.creatureFear = prefab.EnsureComponent<CreatureFear>();
        fleeScared.swimVelocity = swimVelocity * 1.25f;


        WorldSoundPlayer.Add(prefab, TutelLoader.WorldSounds);

		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Acid, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Cold, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Fire, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Poison, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Radiation, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Starve, 0f);

        prefab.FindChild("WM").AddComponent<AnimateByVelocity>().enabled = false; // fixes Aquarium

		yield break;
	}

	public override void ApplyMaterials(GameObject prefab) => MaterialUtils.ApplySNShaders(prefab, 1f);
}
