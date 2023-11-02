namespace SCHIZO.Interop.Subnautica;

partial class _LandCreatureGravity :
#if BELOWZERO
    LandCreatureGravity;
#else
    UnityEngine.MonoBehaviour;
#endif
