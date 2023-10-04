using System.Collections.Generic;
using ECCLibrary.Data;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using SCHIZO.Unity.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

[LoadCreature]
public sealed class ErmfishLoader : PickupableCreatureLoader<PickupableCreatureData, ErmfishPrefab, ErmfishLoader>
{
    // todo: test BZ bus path when SoundPlayers are fixed
    public static readonly SoundPlayer PlayerDeathSounds = new(ResourceManager.LoadAsset<BaseSoundCollection>("Ermfish Player Death"),
        IS_BELOWZERO ? "bus:/master/SFX_for_pause" : "bus:/master/SFX_for_pause/nofilter");

    public ErmfishLoader() : base(ResourceManager.LoadAsset<PickupableCreatureData>("Ermfish data"))
    {
        PDAEncyPath = "Lifeforms/Fauna/SmallHerbivores";
        VariantsAreAlive = true;
        VFXFabricatingData = new VFXFabricatingData("VM/model", -0.255f, 0.67275f, new Vector3(0, 0.22425f), 0.1f, new Vector3(0, -180, 0));
    }

    protected override ErmfishPrefab CreatePrefab()
    {
        return new ErmfishPrefab(ModItems.Ermfish, ModItems.CookedErmfish, ModItems.CuredErmfish, ((CustomCreatureData) creatureData).regularPrefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.025f };
            yield return new LootDistributionData.BiomeData { biome = biome, count = 5, probability = 0.010f };
        }
    }

    protected override void PostRegisterAlive(ModItem item)
    {
        base.PostRegisterAlive(item);
        ItemActionHandler.RegisterMiddleClickAction(item, _ => creatureSounds.AmbientItemSounds.Play2D(), "pull ahoge", "English");
    }
}
