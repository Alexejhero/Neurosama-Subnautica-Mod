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
        [ExposedType("LiveMixin"), SerializeField, UsedImplicitly, Required]
        private MonoBehaviour liveMixin;
        [SerializeField, UsedImplicitly]
        private Item creature;
        [SerializeField, UsedImplicitly]
        private float daysBeforeHatching = 1f;
        [SerializeField, UsedImplicitly, Required]
        private Animator animator;
    }
}
