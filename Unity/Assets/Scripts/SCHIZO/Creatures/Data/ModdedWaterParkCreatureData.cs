using SCHIZO.Items.Data.Crafting;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Water Park Creature Data")]
    public sealed partial class ModdedWaterParkCreatureData : ScriptableObject
    {
        public float initialSize = 0.1f;
        public float maxSize = 0.6f;
        public float outsideSize = 1;
        public float daysToGrow = 1;
        [InfoBox("If false, destroys Pickupable component on dropping outside.", TriMessageType.Warning)]
        public bool isPickupableOutside = true;
        public bool canBreed = true;
        [InfoBox("Spawned when creatures breed. Useless otherwise.")]
        public Item eggOrChild;
        [InfoBox("Spawned when egg/child grows up.")]
        public Item adult;
    }
}
