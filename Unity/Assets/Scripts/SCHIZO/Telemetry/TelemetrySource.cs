using UnityEngine;

namespace SCHIZO.Telemetry
{
    [RequireComponent(typeof(TelemetryCoordinator))]
    public abstract partial class TelemetrySource : MonoBehaviour
    {
        public string category;
        // ReSharper disable once Unity.RedundantEventFunction
        protected virtual void OnDisable() { }
    }
}
