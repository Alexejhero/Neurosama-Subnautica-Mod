using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    [DisallowMultipleComponent]
    public sealed partial class Ermcon : GameEvent
    {
        public int minAttendance = 10;
        public int maxAttendance = 50;
        public float attendeeSearchRadius = 250f;
        public float panelistSearchRadius = 30;
        public float eventDuration = 120f;
        public float cooldown = 1800f;
    }
}
