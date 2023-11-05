using System;
using System.Diagnostics;
using UnityEngine;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Field)]
[Conditional("UNITY_STANDALONE")]
internal sealed class ExposedTypeAttribute(string typeName) : PropertyAttribute
{
    public readonly string typeName = typeName;
}
