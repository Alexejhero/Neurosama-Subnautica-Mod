using SCHIZO.TriInspector.Attributes;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareComponentReferencesGroup]
    [DeclareUnexploredGroup]
    partial class _ModelPlug : MonoBehaviour
    {
        protected const string MODEL_PLUG_GROUP = "Model Plug";

        public Transform plugOrigin;
    }
}
