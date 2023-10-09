using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

public class ErmfishPrefab : PickupableCreaturePrefab<Creature>
{
    private const float _swimVelocity = 7f;

    public ErmfishPrefab(ModItem regular, ModItem cooked, ModItem cured, GameObject prefab) : base(regular, cooked, cured, prefab)
    {
        BehaviourType = BehaviourType.SmallFish;
        EcoTargetType = EcoTargetType.SmallFish;
        MaxHealth = float.MaxValue;

        FoodValueRaw = 9;
        WaterValueRaw = -7;

        FoodValueCooked = 19;
        WaterValueCooked = 0;

        Mass = 10;
        BioReactorCharge = 0;
        SwimRandomData = new SwimRandomData(0.2f, _swimVelocity, new Vector3(20, 5, 20));
        StayAtLeashData = new StayAtLeashData(0.6f, _swimVelocity * 1.25f, 14f);
        ScareableData = new ScareableData();
        FleeWhenScaredData = new FleeWhenScaredData(0.8f, _swimVelocity);
        PickupableFishData = new PickupableFishData(TechType.Floater, "WM", "VM");
        AvoidObstaclesData = new AvoidObstaclesData(1f, _swimVelocity, false, 5f, 5f);
        SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f));
        AnimateByVelocityData = new AnimateByVelocityData(_swimVelocity);
        SwimInSchoolData = new SwimInSchoolData(0.5f, _swimVelocity, 2f, 0.5f, 1f, 0.1f, 25f);
        WaterParkCreatureData = new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID);
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

        prefab.EnsureComponentFields();

        yield break;
    }
}
