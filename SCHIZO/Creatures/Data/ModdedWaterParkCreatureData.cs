using Nautilus.Extensions;
using UnityEngine;

namespace SCHIZO.Creatures.Data;

partial class ModdedWaterParkCreatureData
{
    public WaterParkCreatureData GetData()
    {
        WaterParkCreatureData data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
        data.initialSize = initialSize;
        data.maxSize = maxSize;
        data.outsideSize = outsideSize;
        data.daysToGrow = daysToGrow;
        data.isPickupableOutside = isPickupableOutside;
        data.canBreed = canBreed;
        string eggId = eggOrChild?.GetClassID();
        string adultId = adult?.GetClassID();
        data.eggOrChildPrefab = string.IsNullOrEmpty(eggId) ? null : new(eggId);
        data.eggOrChildPrefab?.ForceValid();
        data.adultPrefab = string.IsNullOrEmpty(adultId) ? null : new(adultId);
        data.adultPrefab?.ForceValid();

        return data;
    }
}
