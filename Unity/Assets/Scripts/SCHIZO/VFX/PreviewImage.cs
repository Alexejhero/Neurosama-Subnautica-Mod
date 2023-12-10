using System;
using UnityEngine;

namespace SCHIZO.VFX
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PreviewImageAttribute() : PropertyAttribute
    {
    }
}
