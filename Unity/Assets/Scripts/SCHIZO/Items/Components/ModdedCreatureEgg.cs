using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Items.Data.Crafting;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Components
{
    [DisallowMultipleComponent]
    public sealed partial class ModdedCreatureEgg : MonoBehaviour
    {
        [ExposedType("CreatureEgg"), SerializeField, UsedImplicitly, Required]
        private MonoBehaviour creatureEgg;
        [SerializeField, UsedImplicitly]
        private Item creature;
    }
}
