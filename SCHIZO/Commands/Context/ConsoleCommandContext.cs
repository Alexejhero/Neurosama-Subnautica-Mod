using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.Context;
internal class ConsoleCommandContext : CommandExecutionContext
{
    public Input.ConsoleInput ConsoleInput
    {
        get => (Input.ConsoleInput)Input;
        init => Input = value;
    }

    public override CommandExecutionContext GetSubContext(Command subCommand)
    {
        return new ConsoleCommandContext()
        {
            Command = subCommand,
            Input = ConsoleInput.GetSubCommandInput(subCommand),
            Output = new(Output.Sinks)
        };
    }
}
