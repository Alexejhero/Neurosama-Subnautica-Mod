using SCHIZO.Creatures.Components;
using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    public sealed partial class ErmconAttendee : CustomCreatureAction
    {
        public float startingEngagement = 10f;
        [Range(0,2f)]
        public float patienceMultiplier = 1f;
    }
}
