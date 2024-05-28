using System;
using System.Reflection;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;

public abstract class Command
{
    /// <summary>
    /// Identifier for the command. If registered as a console command, this is the command trigger.
    /// </summary>
    public string Name { get; protected internal set; }
    /// <summary>
    /// Reader-friendly name for the command.
    /// </summary>
    public string DisplayName { get; protected internal set; }
    /// <summary>
    /// Short description for the command.
    /// </summary>
    public string Description { get; protected internal set; }
    /// <summary>
    /// Additional remarks about the command. Shown when printing usage.
    /// </summary>
    public string Remarks { get; protected internal set; }

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
        object result = ctx.Result;
        ctx.Output.ProcessOutput(ref result);
        switch (ctx.Result)
        {
            case null or CommonResults.OKResult:
                break;
            case CommonResults.ExceptionResult(Exception ex):
                break; // todo
            case CommonResults.ShowUsageResult:
                break; // todo
        }
    }

    protected abstract object ExecuteCore(CommandExecutionContext ctx);

    public static Command FromStaticMethod(MethodInfo method)
        => new MethodCommand(method);
    public static Command FromInstanceMethod(MethodInfo method, object instance)
        => new MethodCommand(method, instance);

    internal void SetInfo(CommandAttribute commandAttr)
        => SetInfo(commandAttr.Name, commandAttr.DisplayName, commandAttr.Description, commandAttr.Remarks);

    internal void SetInfo(string name, string dispName, string desc, string remarks)
    {
        Name = name;
        DisplayName = dispName;
        Description = desc;
        Remarks = remarks;
    }

    internal void SetInfo(SubCommandAttribute subCommandAttr, string methodName)
    {
        Name = !string.IsNullOrEmpty(subCommandAttr.NameOverride)
            ? subCommandAttr.NameOverride
            : methodName;
        DisplayName = subCommandAttr.DisplayName;
        Description = subCommandAttr.Description;
    }
}
