using ECCLibrary.Data;
using UnityEngine;

namespace SCHIZO.Creatures.Components;

partial class ModdedWaterParkCreature
{
    private void Awake()
    {
        WaterParkCreatureData data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
        data.initialSize = initialSize;
        data.maxSize = maxSize;
        data.outsideSize = outsideSize;
        data.daysToGrow = daysToGrow;
        data.isPickupableOutside = isPickupableOutside;
        data.canBreed = canBreed;
        data.eggOrChildPrefab = !string.IsNullOrWhiteSpace(eggOrChildPrefabClassId) ? new CustomGameObjectReference(eggOrChildPrefabClassId) : null;
        data.adultPrefab = !string.IsNullOrWhiteSpace(adultPrefabClassId) ? new CustomGameObjectReference(adultPrefabClassId) : null;

        WaterParkCreature wpc = gameObject.AddComponent<WaterParkCreature>();
        wpc.data = data;
    }
}
