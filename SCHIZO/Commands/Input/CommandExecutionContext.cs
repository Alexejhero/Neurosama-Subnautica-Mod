using System;
using System.Collections.Generic;

namespace SCHIZO.Commands.Input;

// it's abstract because different execution sources have different ways of formatting arguments
// (e.g. console is just one big ol' string, and web is JSON)
public abstract class CommandExecutionContext
{
    public Command RootCommand { get; }
    public abstract IEnumerable<object> Arguments { get; }

    public abstract void SetResult(object result);
    public abstract void SetError(Exception e);

    public abstract CommandExecutionContext GetSubContext(Command subCommand);

    public bool IsTopLevel(Command command) => RootCommand == command;
}
