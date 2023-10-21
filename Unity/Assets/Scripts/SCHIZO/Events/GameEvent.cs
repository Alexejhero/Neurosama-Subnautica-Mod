using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Events
{
    [RequireComponent(typeof(GameEventsConfig))]
    public abstract partial class GameEvent : MonoBehaviour
    {
        [Tooltip("If unchecked, the event can only be started manually through the 'event' console command.")]
        public bool canAutoStart;
        [BoxGroup("Subnautica"), SerializeField, UsedImplicitly]
        [ShowIf(nameof(canAutoStart)), Label("Required Story Goal")]
        private string requiredStoryGoal_SN;
        [BoxGroup("Below Zero"), SerializeField, UsedImplicitly]
        [ShowIf(nameof(canAutoStart)), Label("Required Story Goal")]
        private string requiredStoryGoal_BZ;
    }
}

