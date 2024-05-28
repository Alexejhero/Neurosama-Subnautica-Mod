using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nautilus.Commands;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;
internal class MethodCommand : Command
{
    private readonly ConsoleCommand _proxy;
    private readonly bool _lastTakeAll;
    public IReadOnlyList<ParameterInfo> Parameters { get; protected set;}

    public MethodCommand(MethodInfo method, object instance = null)
    {
        if (method.IsStatic && instance is { })
            throw new ArgumentException("Static method does not need instance");
        else if (!method.IsStatic && instance is null)
            throw new ArgumentException("Non-static method needs instance");
        else if (method.Name.IndexOf('>') >= 0) // mangled (delegate, lambda, inner function, etc...)
            throw new ArgumentException($"Use {nameof(DelegateCommand)} instead of {nameof(MethodCommand)} for delegate methods (lambda, inner function, etc.)");
        _proxy = new("", method, instance: instance);
        Parameters = method.GetParameters();
        if (Parameters is null or { Count: 0 }) return;

        ParameterInfo lastParam = Parameters[^1];
        if (lastParam.ParameterType == typeof(string) && lastParam.GetCustomAttribute<TakeAllAttribute>() is { })
            _lastTakeAll = true;
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        // todo: do better (e.g. named parameters from JSON, remove unnecessary joining/splitting strings, parse enums, parse successive floats as vectors, etc.)
        // it just needs a ton of code practically copied from how nautilus parses commands/args (because it already does like 80% of what we need)
        // which as a practice is meh at best
        int paramCount = Parameters.Count;

        string fullString = ctx.Input.AsConsoleString();
        int firstSpace = fullString.IndexOf(' ');
        IReadOnlyList<string> args;
        if (firstSpace >= 0)
        {
            string argsString = fullString[(firstSpace + 1)..];
            args = argsString.Split(' ');
            if (_lastTakeAll && args.Count > paramCount)
            {
                args = args.Take(paramCount - 1)
                    .Append(string.Join(" ", args.Skip(paramCount - 1)))
                    .ToArray();
            }
        }
        else
        {
            args = [];
        }

        // specifically reimplementing this would be a massive pain
        (int consumed, int parsed) = _proxy.TryParseParameters(args, out object[] parsedArgs);
        bool consumedAll = consumed >= args.Count;
        bool parsedAll = parsed == paramCount;
        if (!consumedAll || !parsedAll)
            return CommonResults.ShowUsage();
            //throw new InvalidOperationException("placeholder message for MethodCommand arg parsing failure");
        return _proxy.Invoke(parsedArgs);
    }
}
