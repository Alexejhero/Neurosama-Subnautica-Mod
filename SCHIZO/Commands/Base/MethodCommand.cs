using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;
#nullable enable
internal class MethodCommand : Command, IParameters
{
    private readonly MethodInfo _method;
    private readonly object? _instance;
    private readonly bool _lastTakeAll;
    public IReadOnlyList<Parameter> Parameters { get; protected set; }

    public MethodCommand(MethodInfo method, object? instance = null)
    {
        if (method.IsStatic && instance is { })
            throw new ArgumentException("Static method does not need instance");
        if (!method.IsStatic && instance is null)
            throw new ArgumentException("Non-static method needs instance");
        if (method.Name.IndexOf('>') >= 0) // mangled (delegate, lambda, inner function, etc...)
            throw new ArgumentException($"Use {nameof(DelegateCommand)} instead of {nameof(MethodCommand)} for delegate methods (lambda, inner function, etc.)");
        _method = method;
        _instance = instance;
        ParameterInfo[] paramInfos = method.GetParameters();

        Parameters = paramInfos
            .Select(pi => new Parameter(pi))
            .ToArray();
        if (paramInfos.Length == 0) return;

        ParameterInfo lastParam = paramInfos[^1];
        if (lastParam.ParameterType == typeof(string) && lastParam.GetCustomAttribute<TakeRemainingAttribute>() is { })
            _lastTakeAll = true;
    }

    internal readonly record struct ArgParseResult(bool ConsumedAllArgs, bool ParsedAllParams, object?[] ParsedArgs);

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        ArgParseResult res = TryParseArgs(ctx);
        if (!res.ConsumedAllArgs || !res.ParsedAllParams)
            return CommonResults.ShowUsage();
            //throw new InvalidOperationException("placeholder message for MethodCommand arg parsing failure");
        try
        {
            return _method.Invoke(_instance, res.ParsedArgs);
        }
        catch (TargetInvocationException e) // very "helpful" wrapper
        {
            return CommonResults.Exception(e.InnerException);
        }
    }

    // todo: pull all of these out
    protected virtual ArgParseResult TryParseArgs(CommandExecutionContext ctx)
    {
        return ctx.Input switch
        {
            StringInput consoleInput => TryParsePositionalArgs(consoleInput.GetPositionalArguments().Cast<string>().ToList(), Parameters, _lastTakeAll),
            RemoteInput jsonInput => TryParseNamedArgs(jsonInput.Model.Args, Parameters),
            _ => throw new InvalidOperationException("Unsupported command input type"),
        };
    }

    internal static ArgParseResult TryParseNamedArgs(Dictionary<string, object?>? args, IReadOnlyList<Parameter> parameters)
    {
        int paramCount = parameters.Count;

        if (args is null)
            return new(true, parameters.All(p => p.IsOptional), []);

        object?[] parsedArgs = new object?[paramCount];
        for (int i = 0; i < paramCount; i++)
        {
            parsedArgs[i] = parameters[i].DefaultValue;
        }
        int consumed = 0;
        int parsed = 0;
        NamedArgs argsButBetter = new(args);
        for (int i = 0; i < paramCount; i++)
        {
            Parameter param = parameters[i];
            if (argsButBetter.TryGetValue(param.Name, out object? obj)
                && Conversion.TryParseOrConvert(obj, param.Type, out object? value))
            {
                parsedArgs[i] = value;
                consumed++;
                parsed++;
            }
            else if (param.IsOptional)
            {
                // parsedArgs[i] = param.DefaultValue; // already default from above init
                parsed++;
            }
            else
            {
                break;
            }
        }
        return new(consumed == args.Count, parsed == parameters.Count, parsedArgs);
    }

    internal static ArgParseResult TryParsePositionalArgs(IReadOnlyList<string>? args, IReadOnlyList<Parameter> parameters, bool lastTakeAll = false)
    {
        if (args is null)
            return new(true, parameters.All(p => p.IsOptional), []);

        int paramCount = parameters.Count;
        if (lastTakeAll && args.Count > paramCount)
        {
            args = args.Take(paramCount - 1)
                .Append(string.Join(" ", args.Skip(paramCount - 1)))
                .ToArray();
        }

        int consumed = 0, parsed = 0;

        object?[] parsedArgs = new object?[paramCount];
        for (int i = 0; i < paramCount; i++)
        {
            parsedArgs[i] = parameters[i].DefaultValue;
        }
        for (int i = 0; i < paramCount; i++)
        {
            Parameter param = parameters[i];
            if (i < args.Count)
            {
                if (!Conversion.TryParseOrConvert(args[i], param.Type, out object? parsedVal))
                    break;
                parsedArgs[i] = parsedVal;
                consumed++;
                parsed++;
            }
            else if (param.IsOptional)
            {
                // parsedArgs[i] = param.DefaultValue; // set above
                parsed++;
            }
            else
            {
                break;
            }
        }

        return new(consumed == args.Count, parsed == parameters.Count, parsedArgs);
    }
}
