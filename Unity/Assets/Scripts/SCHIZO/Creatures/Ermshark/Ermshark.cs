using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Attributes.Typing;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class Ermshark : _Creature
    {
        [BoxGroup("Ermshark"), SerializeField, UsedImplicitly]
        private int mitosisRemaining = 4;

        [BoxGroup("Ermshark"), Required, SerializeField, UsedImplicitly]
        private HurtSoundPlayer hurtSoundPlayer;

        [BoxGroup("Ermshark"), Required, SerializeField, UsedImplicitly]
        private WorldAmbientSoundPlayer ambientSoundPlayer;

        [BoxGroup("Ermshark"), Required, SerializeField, ExposedType("Locomotion"), UsedImplicitly]
        private MonoBehaviour locomotion;
    }
}
