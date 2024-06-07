using System;
using System.Reflection;
using System.Text.RegularExpressions;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Context;

namespace SCHIZO.Commands.Base;

#nullable enable
/// <summary>
/// Represents a triggerable arbitrary action.
/// </summary>
public abstract class Command
{
    /// <summary>
    /// Identifier for the command. If registered as a console command, this is the command trigger.
    /// </summary>
    public string Name { get; protected internal set; } = null!;
    /// <summary>
    /// Reader-friendly name for the command.
    /// </summary>
    public string? DisplayName { get; protected internal set; }
    /// <summary>
    /// Short description for the command.
    /// </summary>
    public string? Description { get; protected internal set; }
    /// <summary>
    /// Additional remarks about the command. Shown when printing usage.
    /// </summary>
    public string? Remarks { get; protected internal set; }

    public void Execute(CommandExecutionContext ctx)
    {
        try
        {
            ctx.SetResult(ExecuteCore(ctx));
        }
        catch (Exception e)
        {
            ctx.SetError(e);
        }
        object? result = ctx.Result;
        ctx.Output.ProcessOutput(ref result);
    }

    protected abstract object? ExecuteCore(CommandExecutionContext ctx);

    public static Command FromStaticMethod(MethodInfo method)
        => new MethodCommand(method);
    public static Command FromInstanceMethod(MethodInfo method, object instance)
        => new MethodCommand(method, instance);

    internal void SetInfo(CommandAttribute commandAttr)
        => SetInfo(commandAttr.Name, commandAttr.DisplayName, commandAttr.Description, commandAttr.Remarks);

    private static readonly Regex _commandNameRegex = new("^[A-Z_][A-Z0-9_]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    internal void SetInfo(string name, string? dispName = null, string? desc = null, string? remarks = null)
    {
        if (name.Length == 0)
            throw new ArgumentException("Command name cannot be empty", nameof(name));
        if (!_commandNameRegex.IsMatch(name))
            throw new ArgumentException($"Command name '{name}' is invalid - only alphanumeric and _ allowed (cannot start with a number)", nameof(name));
        Name = name;
        DisplayName = dispName;
        Description = desc;
        Remarks = remarks;
    }

    internal void SetInfo(SubCommandAttribute subCommandAttr, string methodName)
    {
        SetInfo(name: !string.IsNullOrEmpty(subCommandAttr.NameOverride)
            ? subCommandAttr.NameOverride
            : methodName,
        dispName: subCommandAttr.DisplayName,
        desc: subCommandAttr.Description,
        remarks: null);
    }

    protected Action? PostRegister;
    protected internal void CallPostRegister()
    {
        PostRegister?.Invoke();
    }
}
