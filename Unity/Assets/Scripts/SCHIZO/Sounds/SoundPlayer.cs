using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Sounds
{
    public abstract partial class SoundPlayer : MonoBehaviour
    {
        [SerializeField, Required, UsedImplicitly]
        protected BaseSoundCollection soundCollection;

        [SerializeField, Dropdown(nameof(buses)), ShowIf(nameof(NeedsBus)), UsedImplicitly]
        protected string bus;

        [SerializeField, Required, ExposedType("FMOD_CustomEmitter"), ShowIf(nameof(NeedsEmitter)), UsedImplicitly]
        private MonoBehaviour emitter;

        protected abstract string DefaultBus { get; }
        protected abstract bool Is3D { get; }

        private bool NeedsBus => string.IsNullOrEmpty(DefaultBus);
        private bool NeedsEmitter => Is3D;

        [StaticHelpers.Cache]
        private protected static readonly BetterDropdownList<string> buses = new BetterDropdownList<string>()
        {
            {PDA_VOICE, "Nautilus.Utility.AudioUtils+BusPaths:PDAVoice"},
            {UNDERWATER_CREATURES, "Nautilus.Utility.AudioUtils+BusPaths:UnderwaterCreatures"},
            {INDOOR_SOUNDS, $"SCHIZO.Sounds.{nameof(SoundPlayer)}:{nameof(INDOOR_SOUNDS_BUS)}"}
        };

        protected const string PDA_VOICE = "PDA Voice";
        protected const string UNDERWATER_CREATURES = "Underwater Creatures";
        protected const string INDOOR_SOUNDS = "Indoor Sounds";

        private const string INDOOR_SOUNDS_BUS = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds";
    }
}
