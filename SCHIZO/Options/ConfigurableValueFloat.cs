using System;
using UnityEngine;

namespace SCHIZO.Options;

partial class ConfigurableValueFloat
{
    public override float GetValue()
    {
        return calculateMode switch
        {
            CalculateMode.OneOf => base.GetValue(),
            CalculateMode.Min => Mathf.Min(value, modOption.Value),
            CalculateMode.Max => Mathf.Max(value, modOption.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
