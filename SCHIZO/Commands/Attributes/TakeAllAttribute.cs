using System;

namespace SCHIZO.Commands.Attributes;

/// <summary>
/// When called using a positional input like <see cref="ConsoleInput"/> (e.g. the dev console), take the rest of the input string as this argument.
/// </summary>
/// <remarks>Only valid on the last argument, and it has to be of <see cref="string"/> type.</remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public class TakeAllAttribute : Attribute;
