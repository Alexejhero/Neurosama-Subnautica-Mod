using System;
using SCHIZO.Commands.Context;

namespace SCHIZO.Commands.Base;
internal class DelegateCommand : Command
{
    private readonly Func<object> _func;

    public DelegateCommand(Action action) => _func = () => { action(); return null; };
    public DelegateCommand(Func<object> func) => _func = func;

    protected override object ExecuteCore(CommandExecutionContext ctx)
        => _func();
}
