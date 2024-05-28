using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Output;

public sealed class CommandOutputStack
{
    public static Stack<ISink> GlobalSinks { get; } = new([
        NullSink.Instance,
        LogSink.Message,
    ]);

    /// <summary>
    /// Output stack that shows a UI message for the output like Nautilus but can suppress it if called silently.
    /// </summary>
    public static CommandOutputStack ConsoleSelf = new([UiMessageSink.Sneaky]);

    public CommandOutputStack() { }
    public CommandOutputStack(params ISink[] sinks) : this(sinks.AsEnumerable()) { }
    public CommandOutputStack(IEnumerable<ISink> sinks)
    {
        Sinks.PushRange(sinks);
    }

    public void Push(ISink sink) => Sinks.Push(sink);
    public void Push(CommandOutputStack other)
    {
        // other's sinks will get processed first
        foreach (ISink sink in other.Sinks)
            Sinks.Push(sink);
        //Sinks = new(other.Sinks.Concat(Sinks));
    }

    public Stack<ISink> Sinks { get; } = [];

    public void ProcessOutput(ref object output)
    {
        foreach (ISink sink in Sinks.Concat(GlobalSinks))
        {
            if (sink.TryConsume(ref output))
                break;
        }
    }

    // these feel more like extension methods
    // ok for prototype i suppose, todo refactor

    /// <summary>
    /// Modify the output stack for Nautilus-registered console commands.<br/>
    /// Replaces the current stack contents with a single <see cref="ISink"/> that will set the command <see cref="CommandExecutionContext.Result">result</see>.<br/>
    /// Then, the <see cref="CommandExecutionContext.Result">result</see> is ready for the command to return back to Nautilus, which will handle logging and announcing the output.
    /// </summary>
    public void ModifyForConsoleNautilus(CommandExecutionContext ctx)
    {
        Sinks.Clear();
        Sinks.Push(new SetResultSink(ctx));
    }

    /// <summary>
    /// Adds transformers for <see cref="CommonResults"/> to this stack.
    /// </summary>
    /// <param name="command">Needed to determine usage for <see cref="CommonResults.ShowUsageResult"/>.</param>
    public void AddCommonResultTransformers(Command command)
    {
        Sinks.Push(new LogExceptionSink());
        Sinks.Push(new ShowUsage(command));
    }
}
