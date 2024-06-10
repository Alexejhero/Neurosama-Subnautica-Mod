using System.Linq;
using Newtonsoft.Json;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;

namespace SCHIZO.SwarmControl;
[Command(Name = "sc_export",
    RegisterConsoleCommand = true)]
internal class ExportCommands : Command
{
    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        var allCommands = CommandRegistry.Commands.Values
            .SelectMany(c => c is CompositeCommand comp ? comp.SubCommands.Values.Cast<Command>() : [c]);
        var json = JsonConvert.SerializeObject(allCommands);
        LOGGER.LogInfo(json);

        return CommonResults.OK();
    }
}
