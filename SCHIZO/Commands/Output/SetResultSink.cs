using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.Output;
public sealed class SetResultSink(CommandExecutionContext ctx) : ISink
{
    public bool TryConsume(ref object output)
    {
        ctx.SetResult(output);
        return true;
    }
}
