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

	public override CreatureTemplate CreateTemplate()
	{
		const float swimVelocity = 1.5f;

        // Not yet sure how to make this a creature that "sinks" and dwells the bottom floor
		CreatureTemplate template = new(Prefab, BehaviourType.Crab, EcoTargetType.Coral, float.MaxValue)
		{
			CellLevel = LargeWorldEntity.CellLevel.Medium,
			Mass = 10,
			BioReactorCharge = 0,
			EyeFOV = 0,
			SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(20, 5, 20)),
			StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 14f),
			ScareableData = new ScareableData(),
			FleeWhenScaredData = new FleeWhenScaredData(0.8f, swimVelocity),
			PickupableFishData = new PickupableFishData(TechType.Floater, "WM", "VM"),
			EdibleData = new EdibleData(13, -7, false, 1f),
			ScannerRoomScannable = true,
			CanBeInfected = false,
			AvoidObstaclesData = new AvoidObstaclesData(1f, swimVelocity, false, 5f, 5f),
			SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
			AnimateByVelocityData = new AnimateByVelocityData(swimVelocity),
			SwimInSchoolData = new SwimInSchoolData(0.5f, swimVelocity, 1f, 0.5f, 1f, 0.1f, 25f),
            // Walk on surface and swim above water true
            LocomotionData = new LocomotionData(10f, 0.7f, 0f, 0.1f, false, true, true),
		};
		template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));

		return template;
	}

	public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
	{
        prefab.GetComponent<HeldFish>().ikAimLeftArm = true;

		WorldSoundPlayer.Add(prefab, TutelLoader.WorldSounds);

		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Heat, 0f);
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
