using System;
using System.Diagnostics;
using UnityEngine;

namespace SCHIZO.Attributes.Visual
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_STANDALONE")]
    internal sealed class CarefulAttribute : PropertyAttribute
    {
    }
}
