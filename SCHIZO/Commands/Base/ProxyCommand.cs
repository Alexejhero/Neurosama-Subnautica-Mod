using System;
using System.Collections.Generic;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;

namespace SCHIZO.Commands.Base;
#nullable enable
internal abstract class ProxyCommand<T> : Command, IParameters
    where T : Command, IParameters
{
    public T Target { get; private set; }
    public abstract IReadOnlyList<Parameter> Parameters { get; }

    protected ProxyCommand(T target)
    {
        Target = target;
    }

    protected ProxyCommand(string targetName)
    {
        Target = null!;
        OnRegister += () =>
        {
            if (CommandRegistry.TryGetCommand(targetName, out Command targetMaybe)
                && targetMaybe is T targetDefinitely)
            {
                Target = targetDefinitely;
            }
            else
            {
                throw new InvalidOperationException($"Target command {targetName} was not found or not of type {typeof(T).Name}");
            }
        };
    }

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (ctx is not JsonContext jsonCtx)
            throw new NotSupportedException("Only JSON execution context is supported for now");

        JsonContext targetCtx = GetContextForTarget(jsonCtx);
        Target.Execute(targetCtx);
        return targetCtx.Result;
    }
    protected virtual JsonContext GetContextForTarget(JsonContext proxyCtx)
    {
        return new()
        {
            Command = Target,
            Input = new RemoteInput()
            {
                Command = Target,
                Model = proxyCtx.JsonInput.Model with
                {
                    Args = GetTargetArgs(proxyCtx.JsonInput.Model.Args),
                },
            },
            Output = new(proxyCtx.Output.Sinks)
        };
    }
    // here, do things like adding default args
    protected abstract Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs);
}
