using JetBrains.Annotations;
using SCHIZO.Attributes;
using TriInspector;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    [DeclareFoldoutGroup(CREATURE_GROUP, Title = "Creature")]
    public sealed partial class Ermshark : _Creature
    {
        [SerializeField, UsedImplicitly]
        private int mitosisRemaining = 4;

        [Required, SerializeField, UsedImplicitly]
        private HurtSoundPlayer hurtSoundPlayer;

        [Required, SerializeField, UsedImplicitly]
        private WorldAmbientSoundPlayer ambientSoundPlayer;

        [Required, SerializeField, ExposedType("Locomotion"), UsedImplicitly]
        private MonoBehaviour locomotion;
    }
}
