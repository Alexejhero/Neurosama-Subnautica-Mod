namespace SCHIZO.Commands.Output;

/// <summary>
/// Represents a consumer of command output.
/// </summary>
internal interface ISink
{
    /// <summary>
    /// Attempt to consume output from a command.
    /// </summary>
    /// <param name="output"></param>
    /// <returns>Whether the output was consumed, i.e. whether to stop further propagation to other sinks.</returns>
    public bool TryConsume(object output);
}
