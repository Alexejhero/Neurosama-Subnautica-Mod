using System;
using System.Reflection;
using SCHIZO.Commands.Input;

namespace SCHIZO.Commands;

public abstract class Command
{
    public string Name { get; protected set; }
    public string DisplayName { get; protected set; }
    public string Description { get; protected set; }

    public void Execute(CommandExecutionContext ctx)
    {
        try
        {
            ExecuteCore(ctx);
            // todo process common results like ShowUsage etc
        }
        catch (Exception e)
        {
            ctx.SetError(e);
        }
    }

    protected abstract void ExecuteCore(CommandExecutionContext ctx);

    public static Command FromMethod(MethodInfo method)
        => new MethodCommand(method);
    public static Command FromMethod<T>(string name)
        => new MethodCommand(typeof(T).GetMethod(name) ?? throw new ArgumentException($"No method named {name} on type {typeof(T).FullName}, cannot create command"));
}
