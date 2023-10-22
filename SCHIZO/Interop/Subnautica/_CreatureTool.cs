namespace SCHIZO.Interop.Subnautica;

partial class _CreatureTool :
#if BELOWZERO
    CreatureTool;
#else
    DropTool;
#endif
