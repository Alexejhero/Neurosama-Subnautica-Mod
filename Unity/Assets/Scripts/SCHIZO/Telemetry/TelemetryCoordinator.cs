using UnityEngine;

namespace SCHIZO.Telemetry
{
    [DisallowMultipleComponent]
    public partial class TelemetryCoordinator : MonoBehaviour
    {
        [Tooltip("e.g. http://localhost:1234/api/")]
        public string baseUrl;
        public string playerName;

        private void OnDisable() { }
    }
}
