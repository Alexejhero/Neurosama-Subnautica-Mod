using SCHIZO.Helpers;

namespace SCHIZO.Commands.Output;
public sealed class DelegateSink(DelegateSink.TryConsumeDelegate onOutput) : ISink
{
    public delegate bool TryConsumeDelegate(ref object output);
    public TryConsumeDelegate OnOutput = onOutput;

    public bool TryConsume(ref object output)
    {
        if (OnOutput is null)
            return false;
        foreach (TryConsumeDelegate m in OnOutput.Multicast())
        {
            if (m.Invoke(ref output))
                return true;
        }
        return false;
    }
}
