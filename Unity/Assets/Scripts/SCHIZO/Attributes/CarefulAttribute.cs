using System;
using System.Diagnostics;
using UnityEngine;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Field)]
[Conditional("UNITY_STANDALONE")]
internal sealed class CarefulAttribute : PropertyAttribute
{
}