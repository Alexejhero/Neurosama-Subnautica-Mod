using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TechTypeFlagsAttribute : Attribute
    {
        public readonly Flags flags;

        public TechTypeFlagsAttribute(Flags flags)
        {
            this.flags = flags;
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
