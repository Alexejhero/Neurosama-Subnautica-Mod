using System;
using System.Reflection;
using Nautilus.Commands;

namespace SCHIZO.Commands.Base;
internal class MethodCommand : Command
{
    private readonly ConsoleCommand _proxy;

    public MethodCommand(MethodInfo method, object instance = null)
    {
        if (method.IsStatic && instance is { })
            throw new ArgumentException("Static method does not need instance");
        else if (!method.IsStatic && instance is null)
            throw new ArgumentException("Non-static method needs instance");
        else if (method.Name.IndexOf('>') >= 0) // mangled (delegate, lambda, inner function, etc...)
            throw new ArgumentException($"Use {nameof(DelegateCommand)} instead of {nameof(MethodCommand)} for delegate methods (lambda, inner function, etc.)");
        _proxy = new("", method, instance: instance);
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        // todo: do better (e.g. named parameters from JSON, remove unnecessary joining/splitting strings)
        // it just needs a ton of code practically copied from how nautilus parses commands/args (because it's done sensibly enough there)
        // which is meh at best
        string[] args = ctx.Input.AsConsoleString().Split(' ');
        (int consumed, int parsed) = _proxy.TryParseParameters(args, out object[] parsedArgs);
        bool consumedAll = consumed >= args.Length;
        bool parsedAll = parsed == _proxy.Parameters.Count;
        if (!consumedAll || !parsedAll)
            throw new InvalidOperationException("placeholder message for MethodCommand arg parsing failure");
        return _proxy.Invoke(parsedArgs);
    }
}
