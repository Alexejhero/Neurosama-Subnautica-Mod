using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.SwarmControl.Redeems;
using SwarmControl.Models.Game;

namespace SCHIZO.SwarmControl;
[Command(Name = "sc_export",
    RegisterConsoleCommand = true)]
internal class ExportRedeems : Command
{
    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        IEnumerable<Redeem> allRedeems = RedeemRegistry.Redeems.Values
            .Where(r => r.Export);

        var config = new
        {
            Enums = allRedeems
                .SelectMany(r => r.Args)
                .Select(p => p.ActualType)
                .Where(t => t.IsEnum)
                .Distinct()
                .Select(e => new EnumDefinitionModel(e))
                .ToDictionary(e => e.Name, e => e.Values),
            Redeems = allRedeems
                .ToDictionary(r => r.Id),
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
