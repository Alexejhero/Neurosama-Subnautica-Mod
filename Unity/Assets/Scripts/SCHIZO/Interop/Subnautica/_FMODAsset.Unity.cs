using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _FMODAsset : ScriptableObject
    {
        [ShowIf(nameof(ShowDefaultProps))] public string path;
        [ShowIf(nameof(ShowDefaultProps))] public string id;

        protected virtual bool ShowDefaultProps => true;
    }
}
