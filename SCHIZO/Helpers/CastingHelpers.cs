using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Helpers;

public static class CastingHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FMOD_CustomEmitter ToFMODEmitter(this MonoBehaviour monoBehaviour)
    {
        return (FMOD_CustomEmitter) monoBehaviour;
    }
}
