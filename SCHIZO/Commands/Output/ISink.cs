namespace SCHIZO.Commands.Output;

/// <summary>
/// Represents a consumer of command output.
/// </summary>
public interface ISink
{
    /// <summary>
    /// Attempt to consume output from a command.
    /// </summary>
    /// <returns>Whether the output was consumed, i.e. whether to stop further propagation to other sinks.</returns>
    public bool TryConsume(ref object output);
}
