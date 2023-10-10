using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TechTypeFlagsAttribute : Attribute
    {
        public TechTypeFlagsAttribute(Flags flags)
        {
        }
    }

    [Flags]
    public enum Flags
    {
        Subnautica = 1,
        BelowZero = 2,
        BelowZeroObsolete = 4
    }
}
