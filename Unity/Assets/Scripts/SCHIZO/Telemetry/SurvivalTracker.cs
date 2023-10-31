using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Telemetry
{
    public partial class SurvivalTracker : TelemetrySource
    {
        [InfoBox("The first threshold is the 'critical' percentage, and the second is the 'low' percentage")]
        [MinMaxSlider(0, 1)]
        public Vector2 healthThresholds = new Vector2(0.25f, 0.50f);
        [MinMaxSlider(0, 1)]
        public Vector2 foodThresholds = new Vector2(0.10f, 0.30f);
        [MinMaxSlider(0, 1)]
        public Vector2 waterThresholds = new Vector2(0.20f, 0.40f);

        public float notifyLowInterval = 600;
        public float notifyCriticalInterval = 180;
    }
}
