using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;

namespace SCHIZO.Commands.Context;
internal class JsonContext : CommandExecutionContext
{
    public JsonInput JsonInput
    {
        get => (JsonInput) Input;
        init => Input = value;
    }

    public override CommandExecutionContext GetSubContext(Command subCommand)
    {
        return new JsonContext()
        {
            Command = subCommand,
            Input = JsonInput.GetSubCommandInput(subCommand),
            Output = new(Output.Sinks),
        };
    }
}
