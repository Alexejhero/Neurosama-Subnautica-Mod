using System;
using System.Reflection;
using SCHIZO.Commands.Input;

namespace SCHIZO.Commands;
internal class MethodCommand : Command
{
    public MethodInfo Method { get; protected set; }
    public ParameterInfo[] Parameters { get; protected set; }

    public MethodCommand(MethodInfo method)
    {
        Method = method;
        // todo parse parameters
    }

    protected override void ExecuteCore(CommandExecutionContext ctx)
    {
        // todo convert arguments
    }
}
