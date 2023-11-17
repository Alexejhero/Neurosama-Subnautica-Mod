using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Telemetry
{
    public sealed partial class StoryGoals : TelemetrySource<StoryGoals>
    {
        [InfoBox("Goals not listed here will not be sent to the sink.\nUse {0} to substitute the player's name from the Telemetry Coordinator.")]
        [ListDrawerSettings]
        public StringKVP[] goalDescriptions;
    }

    [Serializable]
    public class StringKVP
    {
        public string goal;
        public string description;
    }
}
