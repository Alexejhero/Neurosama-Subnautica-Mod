using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Helpers;

public static class DelegateHelpers
{
    public static IEnumerable<TFunc> Multicast<TFunc>(this TFunc multicast)
        where TFunc : MulticastDelegate
    {
        return multicast.GetInvocationList().Cast<TFunc>();
    }
}
