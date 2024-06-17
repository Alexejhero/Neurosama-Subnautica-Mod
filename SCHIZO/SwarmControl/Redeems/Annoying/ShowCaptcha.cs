using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Annoying;
#nullable enable

[Redeem(
    Name = "redeem_captcha",
    DisplayName = "Show Captcha",
    Description = "Force Vedal to prove he's human"
)]
internal class ShowCaptcha : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");
        if (!Captcha.Instance) return CommonResults.Error("Captcha not found.");

        CoroutineHost.StartCoroutine(Coro());
        return CommonResults.OK();
    }

    private IEnumerator Coro()
    {
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => !Captcha.Instance.IsOpen);
        Captcha.Instance.Open();
    }
}
