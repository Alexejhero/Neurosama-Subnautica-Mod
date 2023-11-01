using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Telemetry
{
    public sealed partial class BarTracker : TelemetrySource
    {
        [Serializable]
        private partial class TrackedBar
        {
            public string barName;
            public string componentTypeName;
            public string valueMemberName;
            public string maxMemberName;
            public float maxValue = 100; // should be hidden if we have a member name but ShowIf/HideIf doesn't work in nested classes so :shrug:
            [InfoBox("The first threshold is the 'critical' percentage, and the second is the 'low' percentage")]
            [MinMaxSlider(0,1)]
            public Vector2 thresholds;
        }
        [ReorderableList, SerializeField]
        private List<TrackedBar> trackedProperties;
    }
}
