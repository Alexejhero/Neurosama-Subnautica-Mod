using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Telemetry
{
    public abstract partial class TelemetrySource : MonoBehaviour
    {
        [Required]
        public TelemetryCoordinator coordinator;
        public string category;

        protected virtual void OnDisable() { }
    }
}
