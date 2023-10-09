using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.Helpers;

public static class CoroutineHelpers
{
    [System.Obsolete]
    public static Coroutine RunWhen(Action func, Func<bool> predicate, float checkInterval = 1f)
    {
        return CoroutineHost.StartCoroutine(RunWhenCoro(func, predicate, checkInterval));

        static IEnumerator RunWhenCoro(Action func, Func<bool> predicate, float checkInterval = 1f)
        {
            while (!predicate())
                yield return checkInterval <= 0 ? null : new WaitForSeconds(checkInterval);
            func();
        }
    }
}
