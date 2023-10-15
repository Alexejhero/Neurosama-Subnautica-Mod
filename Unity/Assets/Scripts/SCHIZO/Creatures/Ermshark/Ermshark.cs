using NaughtyAttributes;
using SCHIZO.Attributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    [ActualType("Creature")]
    public sealed partial class Ermshark : CustomCreature
    {
        [BoxGroup("Ermshark"), Required, SerializeField] private GameObject ermsharkPrefab;
        [BoxGroup("Ermshark")] public int mitosisRemaining = 4;
        [BoxGroup("Ermshark"), Required, SerializeField] private HurtSoundPlayer hurtSoundPlayer;
        [BoxGroup("Ermshark"), Required, SerializeField] private WorldAmbientSoundPlayer ambientSoundPlayer;
        [BoxGroup("Ermshark"), Required, SerializeField, ExposedType("Locomotion")] private MonoBehaviour locomotion;
    }
}
