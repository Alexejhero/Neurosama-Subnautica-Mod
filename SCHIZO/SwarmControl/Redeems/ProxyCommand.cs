using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems;
#nullable enable
internal class ProxyCommand<T>(T target, Dictionary<string, object> prefillArgs) : Command, IParameters
    where T : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => target.Parameters;
    public Dictionary<string, object> PreFilledArgs { get; } = prefillArgs;
    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        // todo unscutf
        target.Execute(ctx);
        return ctx.Result;
    }
    protected MethodCommand.ArgParseResult TryParseArgs(CommandExecutionContext ctx)
    {
        if (ctx.Input is not RemoteInput jsonInput)
            throw new NotSupportedException("Only JSON input is supported for proxy commands (right now)");
        Dictionary<string, object> args = new(PreFilledArgs);
        if (jsonInput.Model.Args is { } givenArgs)
        {
            foreach (KeyValuePair<string, object> pair in givenArgs)
                args[pair.Key] = pair.Value;
        }
        return MethodCommand.TryParseNamedArgs(args, Parameters);
    }
}
