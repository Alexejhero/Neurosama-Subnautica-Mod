namespace SCHIZO.Spawns;

partial class BiomeSpawnRule
{
    public LootDistributionData.BiomeData GetBiomeData(BiomeType biome)
    {
        return new LootDistributionData.BiomeData
        {
            biome = biome,
            count = count,
            probability = probability
        };
    }
}
