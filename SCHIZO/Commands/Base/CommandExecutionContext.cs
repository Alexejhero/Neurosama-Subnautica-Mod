using System;
using System.Collections.Generic;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;

public class CommandExecutionContext
{
    public Command RootCommand { get; init; }
    public CommandInput Input { get; init; }
    // todo output object
    //public CommandOutput Output { get; init; }
    public bool Silent { get; init; }
    public IEnumerable<object> Arguments => Input.Arguments;

    public object Result { get; protected set; }
    private List<Exception> _exceptions;
    public void SetResult(object result) => Result = result;
    public void SetError(Exception e)
    {
        Result = new CommonResults.ExceptionResult(e);
        _exceptions ??= [];
        _exceptions.Add(e);
    }

    public virtual CommandExecutionContext GetSubContext(Command subCommand)
    {
        return new()
        {
            RootCommand = RootCommand,
            Input = Input.GetSubCommandInput(),
            Silent = Silent,
        };
    }
}
