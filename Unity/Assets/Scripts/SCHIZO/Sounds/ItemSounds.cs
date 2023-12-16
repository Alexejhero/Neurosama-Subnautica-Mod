using FMODUnity;
using SCHIZO.Attributes;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [AddComponentMenu("SCHIZO/Sounds/Item Sounds")]
    public sealed partial class ItemSounds : MonoBehaviour
    {
        [Required, ExposedType("Pickupable")] public MonoBehaviour pickupable;
        public _FMOD_CustomEmitter emitter;
        [ExposedType("_PlayerTool")]
        public MonoBehaviour tool;

        // TODO: change these into SoundPlayers? or keep this thing as one big "multi" sound player
        [EventRef] public string pickup;
        [EventRef] public string drop;

        [Space]

        [EventRef] public string draw;
        [EventRef] public string holster;

        [Space]

        [EventRef] public string cook;
        [EventRef] public string eat;
    }
}
