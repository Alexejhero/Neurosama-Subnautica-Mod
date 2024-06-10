using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.SwarmControl.Redeems;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.SwarmControl;
[Command(Name = "sc_export",
    RegisterConsoleCommand = true)]
internal class ExportRedeems : Command
{
    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        Dictionary<string, Redeem> allRedeems = RedeemRegistry.Redeems;

        var config = new
        {
            Enums = allRedeems.Values
                .SelectMany(r => r.Args)
                .Select(p => p.ActualType)
                .Where(t => t.IsEnum)
                .Select(e => new EnumDefinitionModel(e))
                .ToDictionary(e => e.Name, e => e),
            Redeems = allRedeems,
        };
        string json = JsonConvert.SerializeObject(config, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            NullValueHandling = NullValueHandling.Ignore,
        });
        LOGGER.LogInfo(json);

        return CommonResults.OK();
    }
}
