using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Telemetry
{
    public abstract partial class TelemetrySource : MonoBehaviour
    {
        [Required]
        public TelemetrySender sender;
        public string category;
    }
}
