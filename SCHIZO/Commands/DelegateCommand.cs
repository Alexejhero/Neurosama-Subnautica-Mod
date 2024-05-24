using System;
using SCHIZO.Commands.Input;

namespace SCHIZO.Commands;
internal class DelegateCommand : Command
{
    private readonly Func<object> _func;

    public DelegateCommand(Action action) => _func = () => { action(); return null; };
    public DelegateCommand(Func<object> func) => _func = func;

    protected override void ExecuteCore(CommandExecutionContext ctx)
    {
        ctx.SetResult(_func());
    }
}
