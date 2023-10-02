using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Anneel;

public sealed class AnneelPrefab : CustomCreaturePrefab
{
    public AnneelPrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem, creaturePrefab)
    {
    }


    public override CreatureTemplate CreateTemplate()
    {
        const float swimVelocity = 8f;

        CreatureTemplate template = new(creaturePrefab, BehaviourType.MediumFish, EcoTargetType.MediumFish, 200)
        {
            CellLevel = LargeWorldEntity.CellLevel.Medium,
            SwimRandomData = new SwimRandomData(0.2f, swimVelocity, new Vector3(30, 5, 30), 2, 1),
            StayAtLeashData = new StayAtLeashData(0.6f, swimVelocity * 1.25f, 60),
            ScareableData = new ScareableData(),
            FleeWhenScaredData = new FleeWhenScaredData(0.8f, swimVelocity),
            AvoidObstaclesData = new AvoidObstaclesData(0.6f, swimVelocity, false, 5, 5),
            AcidImmune = true,
            Mass = 120,
            EyeFOV = 0,
            LocomotionData = new LocomotionData(10, 0.45f),
            AnimateByVelocityData = new AnimateByVelocityData(swimVelocity * 1.2f),
            BehaviourLODData = new BehaviourLODData(50, 100, 150),
            RespawnData = new RespawnData(false),
        };
        template.WithoutInfection();

        return template;
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        WorldSounds.Add(prefab, CreatureSoundsHandler.GetCreatureSounds(ModItems.Anneel).AmbientWorldSounds);

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
