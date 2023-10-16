using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed partial class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [SerializeField, Required]
        private BaseSoundCollection soundCollection;

        [SerializeField, Dropdown(nameof(bus_Dropdown))]
        private string bus;

        [SerializeField, Required, ExposedType("FMOD_CustomEmitter")]
        private MonoBehaviour emitter;

        [SerializeField, ExposedType("Pickupable")]
        private MonoBehaviour pickupable;

        private DropdownList<string> bus_Dropdown = new DropdownList<string>()
        {
            {"AudioUtils.BusPaths.PDAVoice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"AudioUtils.BusPaths.UnderwaterCreatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"}
        };
    }
}
