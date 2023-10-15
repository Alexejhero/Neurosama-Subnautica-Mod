using NaughtyAttributes;
using SCHIZO.Packages.NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed partial class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [Required] public SoundCollection soundCollection;
        [Dropdown(nameof(bus_Dropdown))] public string bus;
        [Required, ValidateType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
        [ValidateType("Pickupable")] public MonoBehaviour pickupable;

        private DropdownList<string> bus_Dropdown = NAUGHTYATTRIBUTES.BusPathDropdown;
    }
}
