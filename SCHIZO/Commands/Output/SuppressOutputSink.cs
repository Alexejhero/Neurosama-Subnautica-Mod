using SCHIZO.Helpers;

namespace SCHIZO.Commands.Output;

public sealed class SuppressOutputSink : ISink
{
    public bool TryConsume(ref object output)
    {
        if (MessageHelpers.SuppressOutput)
            output = null;

        return false;
    }
}
