using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Commands.Output;

internal class CommandOutputStack
{
    public static Stack<ISink> GlobalSinks { get; } = new([
        NullSink.Instance,
        LogSink.Message,
    ]);

    public Stack<ISink> Sinks { get; } = [];

    public void ProcessOutput(object output)
    {
        foreach (ISink sink in Sinks.Concat(GlobalSinks))
        {
            if (sink.TryConsume(output))
                break;
        }
    }
}
