using System;
using System.Reflection;

namespace SCHIZO.Commands.Base;
internal class MethodCommand : Command
{
    public MethodInfo Method { get; protected set; }
    public ParameterInfo[] Parameters { get; protected set; }

    public MethodCommand(MethodInfo method)
    {
        Method = method;
        // todo parse parameters
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        // todo convert arguments
        return null;
    }
}
