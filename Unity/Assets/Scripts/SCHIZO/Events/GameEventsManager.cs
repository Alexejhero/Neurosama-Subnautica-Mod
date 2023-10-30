using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Events
{
    [DisallowMultipleComponent]
    public sealed partial class GameEventsManager : MonoBehaviour
    {
        [Tooltip("If unchecked, events can only be started via the 'event' console command.")]
        [InfoBox("This is only a default! It has no effect if it's already configured by the player.")]
        public bool autoStartEvents = true;
        [InfoBox("If set, the \"Auto Start Events\" checkbox will override any existing preference. Please don't abuse this.")]
        public bool overridePlayerPrefs;
    }
}
