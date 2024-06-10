using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.ConsoleCommands;

#nullable enable
/// <summary>
/// Wrapper around a base-game console command.
/// </summary>
public abstract class ConsoleWrapperCommand(string commandName) : Command, IParameters
{
    public string Command { get; } = commandName;
    public abstract IReadOnlyList<Parameter> Parameters { get; }

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (!DevConsole.commands.ContainsKey(Command))
            return CommonResults.Error($"Console command {Command} was not found");
        object?[] args = GetArgs(ctx).ToArray();
        int failArgIndex = ValidateArgs(args);
        if (failArgIndex >= 0)
        {
            string failParam = Parameters.ElementAtOrDefault(failArgIndex)?.Name ?? "too many";
            string failVal = args.Length <= failArgIndex ? "no value" : $"value \"{args[failArgIndex]}\"";
            return CommonResults.Error($"Argument {failArgIndex} ({failParam}, {failVal}) failed validation");
        }
        string consoleInput = $"{Command} {string.Join(" ", args)}";
        DevConsole.SendConsoleCommand(consoleInput);
        return null;
    }

    private IEnumerable<object?> GetArgs(CommandExecutionContext ctx)
    {
        // until first null/default
        return ctx.Input.GetPositionalArguments()
            .TakeWhile(arg => arg is { } && arg != DBNull.Value && arg != Type.Missing);
    }

    // todo do better (maybe)
    protected virtual int ValidateArgs(IReadOnlyList<object?> args)
    {
        if (args.Count > Parameters.Count)
            return args.Count;
        if (args.Count < Parameters.Count && !Parameters[args.Count].IsOptional)
            return args.Count;
        for (int i = 0; i < Parameters.Count; i++)
        {
            Parameter param = Parameters[i];
            if (args.Count <= i)
            {
                if (param.IsOptional)
                    break;
                else
                    return i;
            }
            object? arg = args[i];
            if (!param.IsOptional && (arg == DBNull.Value || arg == Type.Missing))
                return i;
            if (!Conversion.TryParseOrConvert(arg, param.Type, out _))
                return i;
        }
        return -1;
    }
}
