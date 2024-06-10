using System;
using System.Collections.Generic;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;
#nullable enable
internal abstract class ProxyCommand<T> : Command, IParameters
    where T : Command
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
        PostRegister += () =>
        {
            if (CommandRegistry.TryGetInnermostCommand(targetName, out Command targetMaybe)
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
        RemoteInput targetInput = new()
        {
            Command = Target,
            Model = proxyCtx.JsonInput.Model with
            {
                Args = GetTargetArgs(proxyCtx.JsonInput.Model.Args),
                Announce = false,
            },
        };
        return new()
        {
            Command = Target,
            Input = targetInput,
            Output = new([
                NullSink.Instance, // stop it before it gets to default sinks (which would log the result twice)
                new DelegateSink((ref object _) => {
                    LOGGER.LogInfo($"{GetType().Name} {targetInput.AsConsoleString()}");
                    return false;
                }),
            ])
        };
    }
    // here, do things like adding default args or overriding/converting to target command's param types
    protected abstract Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs);
}
