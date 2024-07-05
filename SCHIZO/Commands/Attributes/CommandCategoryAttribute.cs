using System;
using JetBrains.Annotations;

namespace SCHIZO.Commands.Attributes;

/// <summary>
/// Register all methods marked with <see cref="CommandAttribute"/> in this class.
/// </summary>
/// <remarks>Methods decorated with <see cref="CommandAttribute"/> must be <see langword="static"/>.</remarks>
/// <param name="category">Category to register the commands to.</param>
[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class CommandCategoryAttribute(string category = "Uncategorized") : Attribute
{
    public string Category { get; set; } = category;
}
