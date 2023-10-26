using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Events
{
    [RequireComponent(typeof(GameEventsManager))]
    public abstract partial class GameEvent : MonoBehaviour
    {
        [Tooltip("If unchecked, the event can only be started manually through the 'event' console command.")]
        public bool canAutoStart;
        [BoxGroup("Subnautica"), SerializeField, ShowIf(nameof(canAutoStart))]
        [Label("Required Story Goal"), Tooltip("This story goal must be completed before the event can autostart.")]
        private string requiredStoryGoal_SN;
        [BoxGroup("Below Zero"), SerializeField, ShowIf(nameof(canAutoStart))]
        [Label("Required Story Goal"), Tooltip("This story goal must be completed before the event can autostart.")]
        private string requiredStoryGoal_BZ;
    }
}

