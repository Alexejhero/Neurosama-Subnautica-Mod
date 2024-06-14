using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

[Redeem(
    Name = "redeem_scanrandomfragment",
    DisplayName = "Scan Random Fragment",
    Description = "Auto-scans a random fragment, chosen by fair dice roll."
)]
internal class ScanTotallyRandomFragment : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        CoroutineHost.StartCoroutine(Coro());
        return CommonResults.OK();
    }

    private IEnumerator Coro()
    {
        yield return new WaitForSeconds(0.5f);
        ErrorMessage.AddMessage("Unlocking random fragment...");
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        ErrorMessage.AddMessage("Result: Thermal Plant Fragment");
        yield return InventoryConsoleCommands.ItemCmdSpawnAsync(2, TechType.Titanium);
    }
}
