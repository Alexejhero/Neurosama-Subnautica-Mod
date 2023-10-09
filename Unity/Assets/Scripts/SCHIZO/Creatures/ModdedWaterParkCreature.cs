using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [DisallowMultipleComponent]
    public sealed class ModdedWaterParkCreature : MonoBehaviour
    {
        public float initialSize = 0.1f;
        public float maxSize = 0.6f;
        public float outsideSize = 1;
        public float daysToGrow = 1;
        public bool isPickupableOutside = true;
        public bool canBreed = true;
        public string eggOrChildPrefabClassId;
        public string adultPrefabClassId;

#if !UNITY
        private void Awake()
        {
            WaterParkCreatureData data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
            data.initialSize = initialSize;
            data.maxSize = maxSize;
            data.outsideSize = outsideSize;
            data.daysToGrow = daysToGrow;
            data.isPickupableOutside = isPickupableOutside;
            data.canBreed = canBreed;
            data.eggOrChildPrefab = !string.IsNullOrWhiteSpace(eggOrChildPrefabClassId) ? new ECCLibrary.Data.CustomGameObjectReference(eggOrChildPrefabClassId) : null;
            data.adultPrefab = !string.IsNullOrWhiteSpace(adultPrefabClassId) ? new ECCLibrary.Data.CustomGameObjectReference(adultPrefabClassId) : null;

            WaterParkCreature wpc = gameObject.AddComponent<WaterParkCreature>();
            wpc.data = data;
        }
#endif
    }
}
