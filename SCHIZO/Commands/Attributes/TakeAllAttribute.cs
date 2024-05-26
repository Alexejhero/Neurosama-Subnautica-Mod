using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHIZO.Commands.Attributes;

/// <summary>
/// When called via the dev console, take the rest of the input string as this argument.
/// </summary>
/// <remarks>Only valid on the last argument, and it has to be of <see cref="string"/> type.</remarks>
[AttributeUsage(AttributeTargets.Parameter)]
internal class TakeAllAttribute : Attribute;
