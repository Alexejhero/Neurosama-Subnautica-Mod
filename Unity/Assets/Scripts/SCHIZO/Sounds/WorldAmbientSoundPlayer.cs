using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed partial class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [Required] public SoundCollection soundCollection;
        [Dropdown(nameof(bus_Dropdown))] public string bus;
        [Required, ExposedType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
        [ExposedType("Pickupable")] public MonoBehaviour pickupable;

        private DropdownList<string> bus_Dropdown = new DropdownList<string>()
        {
            {"AudioUtils.BusPaths.PDAVoice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"AudioUtils.BusPaths.UnderwaterCreatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"}
        };
    }
}
