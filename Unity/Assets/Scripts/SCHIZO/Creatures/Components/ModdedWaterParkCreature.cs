using JetBrains.Annotations;
using SCHIZO.Items.Data.Crafting;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DisallowMultipleComponent]
    public sealed partial class ModdedWaterParkCreature : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private float initialSize = 0.1f;
        [SerializeField, UsedImplicitly] private float maxSize = 0.6f;
        [SerializeField, UsedImplicitly] private float outsideSize = 1;
        [SerializeField, UsedImplicitly] private float daysToGrow = 1;
        [SerializeField, UsedImplicitly] private bool isPickupableOutside = true;
        [SerializeField, UsedImplicitly] private bool canBreed = true;
        [SerializeField, UsedImplicitly] private Item eggOrChild;
        [SerializeField, UsedImplicitly] private Item adult;
    }
}
