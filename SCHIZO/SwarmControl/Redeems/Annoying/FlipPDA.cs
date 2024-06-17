using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(Name = "redeem_flippda",
    DisplayName = "Flip PDA",
    Description = "Oops I forgot how to hold things"
)]
internal class FlipPDA : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");

        PDA pda = Player.main.GetPDA();
        if (!pda) return CommonResults.Error("Could not get PDA."); // should never ever ever happen

        Vector3 pdaScale = pda.transform.localScale;
        pda.transform.localScale = new(pdaScale.x, -pdaScale.y, pdaScale.z);
        Vector3 uiScale = pda.ui.transform.localScale;
        pda.ui.transform.localScale = new(uiScale.x, -uiScale.y, uiScale.z);

        return CommonResults.OK();
    }
}
