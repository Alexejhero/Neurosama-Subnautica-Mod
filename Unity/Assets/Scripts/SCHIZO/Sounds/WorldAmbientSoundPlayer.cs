using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Helpers;
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

        [Space]

        [SerializeField, ExposedType("Pickupable"), UsedImplicitly]
        private MonoBehaviour pickupable;

        [SerializeField, ExposedType("Constructable"), UsedImplicitly]
        private MonoBehaviour constructable;

        [StaticHelpers.Cache]
        private static DropdownList<string> bus_Dropdown = new DropdownList<string>()
        {
            {"PDA Voice", "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {"Underwater Creatures", "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"},
            {"Indoor Sounds", $"SCHIZO.Sounds.{nameof(WorldAmbientSoundPlayer)}:{nameof(_indoorSoundsBus)}"}
        };

        private const string _indoorSoundsBus = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds";
    }
}
