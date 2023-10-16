using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DisallowMultipleComponent]
    public sealed partial class ModdedWaterParkCreature : MonoBehaviour
    {
        [SerializeField] private float initialSize = 0.1f;
        [SerializeField] private float maxSize = 0.6f;
        [SerializeField] private float outsideSize = 1;
        [SerializeField] private float daysToGrow = 1;
        [SerializeField] private bool isPickupableOutside = true;
        [SerializeField] private bool canBreed = true;
        [SerializeField] private string eggOrChildPrefabClassId;
        [SerializeField] private string adultPrefabClassId;
    }
}
