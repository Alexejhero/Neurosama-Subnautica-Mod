using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHIZO.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public required string Name;
    public string DisplayName;
    public string Description;
}
