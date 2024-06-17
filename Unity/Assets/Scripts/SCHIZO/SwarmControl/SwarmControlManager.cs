using TriInspector;
using UnityEngine;

namespace SCHIZO.SwarmControl
{
    public partial class SwarmControlManager : MonoBehaviour
    {
        [InfoBox("This is the default base URL for the server.\nUsers can change this with the sc_url command.")]
        public string defaultBackendUrl;
        [InfoBox("If client and server versions don't match, connecting will fail.")]
        public string version;
    }
}
