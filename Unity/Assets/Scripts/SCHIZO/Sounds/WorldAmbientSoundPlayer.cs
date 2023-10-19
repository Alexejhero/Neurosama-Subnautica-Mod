using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed partial class WorldAmbientSoundPlayer : MonoBehaviour
    {
        [SerializeField, Required, UsedImplicitly]
        private BaseSoundCollection soundCollection;

        [SerializeField, Dropdown(nameof(bus_Dropdown)), UsedImplicitly]
        private string bus;

        [SerializeField, Required, ExposedType("FMOD_CustomEmitter"), UsedImplicitly]
        private MonoBehaviour emitter;

        [SerializeField, ExposedType("Pickupable"), UsedImplicitly]
        private MonoBehaviour pickupable;

        private DropdownList<string> bus_Dropdown = new DropdownList<string>()
        {
            {"PDA Voice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"Underwater Creatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"}
        };
    }
}
