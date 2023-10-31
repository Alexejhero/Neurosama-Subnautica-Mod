using UnityEngine;

namespace SCHIZO.Telemetry
{
    [DisallowMultipleComponent]
    public partial class TelemetrySender : MonoBehaviour
    {
        [Tooltip("e.g. http://localhost:1234/api/")]
        public string baseUrl;
    }
}
