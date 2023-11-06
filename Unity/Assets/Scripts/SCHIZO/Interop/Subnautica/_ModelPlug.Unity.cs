using SCHIZO.TriInspector;
using SCHIZO.TriInspector.Attributes;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareUnexploredGroup(MODEL_PLUG_GROUP)]
    partial class _ModelPlug : TriMonoBehaviour
    {
        protected const string MODEL_PLUG_GROUP = "Model Plug";

        [UnexploredGroup(MODEL_PLUG_GROUP)] public Transform plugOrigin;
    }
}
