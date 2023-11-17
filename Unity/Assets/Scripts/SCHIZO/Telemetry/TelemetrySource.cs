using UnityEngine;

namespace SCHIZO.Telemetry
{
    [RequireComponent(typeof(TelemetryCoordinator))]
    public abstract partial class TelemetrySource<T> : MonoBehaviour
        where T : TelemetrySource<T>
    {
        public string category;
        // ReSharper disable once Unity.RedundantEventFunction
        protected virtual void OnDisable() { }
    }
}
