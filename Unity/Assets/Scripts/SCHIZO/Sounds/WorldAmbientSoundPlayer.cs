using NaughtyAttributes;
using SCHIZO.Unity.NaughtyExtensions;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
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
