using System;
using System.Reflection;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;

public abstract class Command
{
    public string Name { get; protected set; }
    public string DisplayName { get; protected set; }
    public string Description { get; protected set; }

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

    public static Command FromMethod(MethodInfo method)
        => new MethodCommand(method);
    public static Command FromMethod<T>(string name)
        => new MethodCommand(typeof(T).GetMethod(name) ?? throw new ArgumentException($"No method named {name} on type {typeof(T).FullName}, cannot create command"));
}
