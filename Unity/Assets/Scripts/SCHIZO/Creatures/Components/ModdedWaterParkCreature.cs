using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DisallowMultipleComponent]
    public sealed partial class ModdedWaterParkCreature : MonoBehaviour
    {
        public float initialSize = 0.1f;
        public float maxSize = 0.6f;
        public float outsideSize = 1;
        public float daysToGrow = 1;
        public bool isPickupableOutside = true;
        public bool canBreed = true;
        public string eggOrChildPrefabClassId;
        public string adultPrefabClassId;
    }
}
