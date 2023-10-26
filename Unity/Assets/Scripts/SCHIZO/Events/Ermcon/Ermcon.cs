using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    [DisallowMultipleComponent]
    public sealed partial class Ermcon : GameEvent
    {
        [MinMaxSlider(0, 100), Tooltip("Min/max number of creatures attending the event.")]
        public Vector2Int ticketsSold = new Vector2Int(10, 50);
        public float attendeeSearchRadius = 250f;
        public float panelistSearchRadius = 30;
        public float eventDuration = 120f;
        public float cooldown = 1800f;

        public int MinAttendance => ticketsSold.x;
        public int MaxAttendance => ticketsSold.y;
    }
}
