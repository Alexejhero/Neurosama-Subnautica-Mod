using System;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;

namespace SCHIZO.Commands;

[Command(Name = "throw",
    DisplayName = "Throw Exception (Debug)",
    Description = "Throw exception(s) of the given type(s) (for debugging commands)",
    RegisterConsoleCommand = true)]
internal class ThrowCommand : CompositeCommand
{
    [SubCommand("new")]
    public static object Throw(string exceptionTypeName, [TakeRemaining] string message = "")
    {
        Type type = ReflectionCache.GetType(exceptionTypeName);
        if (type is null)
            return $"Could not find type '{exceptionTypeName}'";
        if (!typeof(Exception).IsAssignableFrom(type))
            return $"'{exceptionTypeName}' is not an exception type";
        throw (Exception)Activator.CreateInstance(type, [message]);
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        object result = base.ExecuteCore(ctx);
        if (result is not CommonResults.ExceptionResult)
            return result;
        throw new Exception("Outer exception");
    }
}
