using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHIZO.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public required string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool RegisterConsoleCommand { get; set; }
    public string Remarks { get; set; }
}
