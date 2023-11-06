using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Events
{
    [DeclareBoxGroup("sn", Title = "Subnautica")]
    [DeclareBoxGroup("bz", Title = "Below Zero")]
    public abstract partial class GameEvent : MonoBehaviour
    {
        [Tooltip("If unchecked, the event can only be started manually through the 'event' console command.")]
        public bool canAutoStart = true;

        [Group("sn"), SerializeField, ShowIf(nameof(canAutoStart)), UsedImplicitly]
        [LabelText("Required Story Goal"), Tooltip("This story goal must be completed before the event can autostart.")]
        private string requiredStoryGoal_SN;

        [Group("bz"), SerializeField, ShowIf(nameof(canAutoStart)), UsedImplicitly]
        [LabelText("Required Story Goal"), Tooltip("This story goal must be completed before the event can autostart.")]
        private string requiredStoryGoal_BZ;
    }
}
