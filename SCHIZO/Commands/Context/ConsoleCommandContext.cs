using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.Context;
internal class ConsoleCommandContext : CommandExecutionContext
{
    public Input.StringInput ConsoleInput
    {
        get => (Input.StringInput)Input;
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
