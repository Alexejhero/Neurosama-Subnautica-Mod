using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Context;

#nullable enable
public abstract class CommandExecutionContext
{
    public required Command Command { get; init; }
    public required CommandInput Input { get; init; }
    public required CommandOutputStack Output { get; set; }

    public object? Result { get; protected set; }
    public void SetResult(object? result) => Result = result;
    public void SetError(Exception e)
    {
        if (Result is not CommonResults.ExceptionResult { Exception: { } existing })
        {
            Result = new CommonResults.ExceptionResult(e);
            return;
        }

        IEnumerable<Exception> exceptions = existing is AggregateException agg
            ? [..agg.InnerExceptions, e]
            : [existing, e];
        Result = new AggregateException(exceptions);
    }

    public abstract CommandExecutionContext GetSubContext(Command subCommand);
}
