using NaughtyAttributes;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class Ermshark : CustomCreature
    {
        [BoxGroup("Ermshark"), Required, SerializeField] private Ermshark ermsharkPrefab;
        [BoxGroup("Ermshark")] public int mitosisRemaining = 4;
        [BoxGroup("Ermshark"), Required, SerializeField] private HurtSoundPlayer hurtSoundPlayer;
        [BoxGroup("Ermshark"), Required, SerializeField] private WorldAmbientSoundPlayer ambientSoundPlayer;
        [BoxGroup("Ermshark"), Required, SerializeField, ValidateType("Locomotion")] private MonoBehaviour locomotion;
    }
}
