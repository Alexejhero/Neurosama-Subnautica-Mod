using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

[DeclareBoxGroup("sn", Title = "Subnautica")]
[DeclareBoxGroup("bz", Title = "Below Zero")]
[DeclareComponentReferencesGroup]
public class CreatureEgg : MonoBehaviour
{
        [SerializeField, UsedImplicitly]
        private float daysBeforeHatching = 1f;
        [SerializeField, UsedImplicitly, HideInInspector]
        private AssetReferenceGameObject creaturePrefab;

        [ComponentReferencesGroupNext]
        [ExposedType("LiveMixin"), SerializeField, UsedImplicitly, Required]
        private MonoBehaviour liveMixin;

        [Group("sn"), SerializeField, UsedImplicitly, HideInInspector]
        private Animator animator;
        [Group("bz"), SerializeField, UsedImplicitly, HideInInspector]
        private Animator[] animators;
}
