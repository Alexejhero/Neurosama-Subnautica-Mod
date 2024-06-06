using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nautilus.Commands;
using Nautilus.Extensions;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;
#nullable enable
internal class MethodCommand : Command, IParameters
{
    private readonly ConsoleCommand _proxy;
    private readonly bool _lastTakeAll;
    public IReadOnlyList<Parameter> Parameters { get; protected set; } = [];

    public MethodCommand(MethodInfo method, object? instance = null)
    {
        if (method.IsStatic && instance is { })
            throw new ArgumentException("Static method does not need instance");
        else if (!method.IsStatic && instance is null)
            throw new ArgumentException("Non-static method needs instance");
        else if (method.Name.IndexOf('>') >= 0) // mangled (delegate, lambda, inner function, etc...)
            throw new ArgumentException($"Use {nameof(DelegateCommand)} instead of {nameof(MethodCommand)} for delegate methods (lambda, inner function, etc.)");
        _proxy = new("", method, instance: instance);
        ParameterInfo[] paramInfos = method.GetParameters();

        Parameters = paramInfos
            .Select(pi => new Parameter(pi))
            .ToArray();
        if (paramInfos.Length == 0) return;

        ParameterInfo lastParam = paramInfos[^1];
        if (lastParam.ParameterType == typeof(string) && lastParam.GetCustomAttribute<TakeRemainingAttribute>() is { })
            _lastTakeAll = true;
    }

    internal readonly record struct ArgParseResult(bool ConsumedAllArgs, bool ParsedAllParams, object[] ParsedArgs);

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        ArgParseResult res = TryParseArgs(ctx);
        if (!res.ConsumedAllArgs || !res.ParsedAllParams)
            return CommonResults.ShowUsage();
            //throw new InvalidOperationException("placeholder message for MethodCommand arg parsing failure");
        try
        {
            return _proxy.Invoke(res.ParsedArgs);
        }
        catch (TargetInvocationException e) // very "helpful" wrapper
        {
            return CommonResults.Exception(e.InnerException);
        }
    }

    protected virtual ArgParseResult TryParseArgs(CommandExecutionContext ctx)
    {
        return ctx.Input switch
        {
            StringInput consoleInput => TryParsePositionalArgs(consoleInput.GetPositionalArguments().Cast<string>().ToList()),
            RemoteInput jsonInput => TryParseNamedArgs(jsonInput.Model.Args, Parameters),
            _ => throw new InvalidOperationException("Unsupported command input type"),
        };
    }

    internal static ArgParseResult TryParseNamedArgs(Dictionary<string, object?>? args, IReadOnlyList<Parameter> parameters)
    {
        int paramCount = parameters.Count;

        if (args is null)
            return new(true, paramCount == 0, []);

        List<object?> parsedArgs = [];
        Dictionary<string, object?> argsCopy = new(args);
        List<Parameter> paramsLeft = [.. parameters];
        for (int i = paramCount - 1; i >= 0; i--)
        {
            Parameter param = paramsLeft[i];
            if (argsCopy.TryGetValue(param.Name, out object? value))
            {
                // check value type is nullable
                if (value is null)
                {
                    // allow reference types and nullable value type
                    // forbid non-nullable value types
                    if (param.Type.IsValueType && !param.Type.TryUnwrapNullableType(out _))
                        break;
                }
                else if (value.GetType() != param.Type)
                {
                    break;
                }

                parsedArgs.Add(value);
                argsCopy.Remove(param.Name);
                paramsLeft.RemoveAt(i);
            }
            else if (!param.IsOptional)
            {
                break;
            }
        }
        return new(argsCopy.Count == 0, paramsLeft.Count == 0, [..parsedArgs]);
    }

    internal ArgParseResult TryParsePositionalArgs(IReadOnlyList<string> args)
    {
        int paramCount = Parameters.Count;
        if (_lastTakeAll && args.Count > paramCount)
        {
            args = args.Take(paramCount - 1)
                .Append(string.Join(" ", args.Skip(paramCount - 1)))
                .ToArray();
        }
        // aaa i don't want to reimplement this
        (int consumed, int parsed) = _proxy.TryParseParameters(args, out object[] parsedArgs);
        return new(consumed == args.Count, parsed == Parameters.Count, parsedArgs);
    }
}
