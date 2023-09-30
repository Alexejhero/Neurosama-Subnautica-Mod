using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using Nautilus.Assets;
using SCHIZO.Extensions;
using SCHIZO.Helpers;
using SCHIZO.Sounds;

namespace SCHIZO.Creatures.Ermfish;

public class ErmfishPrefab : CreatureAsset
{
    private readonly GameObject _prefab;

	public ErmfishPrefab(PrefabInfo prefabInfo, GameObject prefab) : base(prefabInfo)
    {
        _prefab = prefab;
    }

	public override CreatureTemplate CreateTemplate()
	{
		const float swimVelocity = 7f;

		CreatureTemplate template = new(_prefab, BehaviourType.SmallFish, EcoTargetType.Peeper, float.MaxValue)
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
			AvoidObstaclesData = new AvoidObstaclesData(1f, swimVelocity, false, 5f, 5f),
			SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
			AnimateByVelocityData = new AnimateByVelocityData(swimVelocity),
			SwimInSchoolData = new SwimInSchoolData(0.5f, swimVelocity, 2f, 0.5f, 1f, 0.1f, 25f),
		};
        template.WithoutInfection();
		template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));

		return template;
	}

	public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
	{
        prefab.GetComponent<HeldFish>().ikAimLeftArm = true;

        CreatureSounds sounds = CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermfish);
		InventorySounds.Add(prefab, sounds.AmbientItemSounds);
		WorldSounds.Add(prefab, sounds.AmbientWorldSounds);

		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Acid, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Cold, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Fire, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Poison, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Radiation, 0f);
		CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Starve, 0f);

        prefab.FindChild("WM").AddComponent<AnimateByVelocity>().enabled = false; // fixes Aquarium

        prefab.EnsureComponentFields();

		yield break;
	}

	public override void ApplyMaterials(GameObject prefab) => MaterialHelpers.ApplySNShadersIncludingRemaps(prefab, 1f);
}
