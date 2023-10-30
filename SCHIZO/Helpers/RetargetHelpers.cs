using System.Runtime.CompilerServices;

namespace SCHIZO.Helpers;

public static class RetargetHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static
#if SUBNAUTICA
        TSubnautica
#else
        TBelowZero
#endif
        Pick<TSubnautica, TBelowZero>(TSubnautica subnautica, TBelowZero belowZero)
    {
#if SUBNAUTICA
        return subnautica;
#else
        return belowZero;
#endif
    }
}
