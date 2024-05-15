using ECCLibrary.Data;
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
        data.eggOrChildPrefab = !string.IsNullOrWhiteSpace(eggOrChild.GetClassID()) ? new CustomGameObjectReference(eggOrChild.GetClassID()) : null;
        data.adultPrefab = !string.IsNullOrWhiteSpace(adult.GetClassID()) ? new CustomGameObjectReference(adult.GetClassID()) : null;

        return data;
    }
}
