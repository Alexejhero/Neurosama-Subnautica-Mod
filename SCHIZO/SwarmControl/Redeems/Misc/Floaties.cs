using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Misc;

#nullable enable
[Redeem(
    Name = "redeem_airbladder",
    DisplayName = "Floaties",
    Description = "Push player up towards the surface for 10 seconds. Does nothing if above water."
)]
internal class Floaties : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];
    private static readonly float buoyancyForce = 50f;
    private static readonly float Duration = 10f;
    private readonly EggTimer _timer;

    public Floaties()
    {
        _timer = new(() => { }, () => { }, FixedUpdate);
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        _timer.AddTime(Duration);
        _timer.Start();
        return CommonResults.OK();
    }

    private void FixedUpdate()
    {
        if (!Player.main) return;
        if (!Player.main.IsUnderwaterForSwimming()) return;

        float depth = Ocean.GetOceanLevel() - 1f - Player.main.transform.position.y;
        Rigidbody rb = Player.main.rigidBody;
        if (rb.velocity.y * Time.fixedDeltaTime < depth)
        {
            Vector3 vector = Vector3.up * buoyancyForce;
            rb.AddForce(vector, ForceMode.Acceleration);
        }
    }
}
