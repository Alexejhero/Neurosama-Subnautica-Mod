using NaughtyAttributes;
using SCHIZO.Attributes;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed partial class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [Required] public SoundCollection soundCollection;
        [Dropdown(nameof(bus_Dropdown))] public string bus;
        [Required, ExposedType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
        [ExposedType("Pickupable")] public MonoBehaviour pickupable;

        private DropdownList<string> bus_Dropdown = NAUGHTYATTRIBUTES.BusPathDropdown;
    }
}
