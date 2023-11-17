using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Anneel;

public sealed class AnneelPrefab : CustomCreaturePrefab<Creature>
{
    private const float _swimVelocity = 8f;

    public AnneelPrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem, creaturePrefab)
    {
        BehaviourType = BehaviourType.MediumFish;
        EcoTargetType = EcoTargetType.MediumFish;
        MaxHealth = 200;

        StayAtLeashData = new StayAtLeashData(0.6f, _swimVelocity * 1.25f, 60);
        AnimateByVelocityData = new AnimateByVelocityData(_swimVelocity * 0.9f);
        BehaviourLODData = new BehaviourLODData(50, 100, 150);
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
