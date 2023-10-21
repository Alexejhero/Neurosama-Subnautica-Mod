using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Events
{
    [DisallowMultipleComponent]
    public sealed partial class GameEventsConfig : MonoBehaviour
    {
        [Tooltip("If unchecked, events can only be started via the 'event' console command.")]
        [InfoBox("This is only the default - it only takes effect if not configured beforehand")]
        public bool autoStartEvents;
    }
}
