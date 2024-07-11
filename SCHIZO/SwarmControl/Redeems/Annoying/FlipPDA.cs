using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using TMPro;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_flippda",
    DisplayName = "Flip PDA",
    Description = "Oops I forgot how to hold things"
)]
internal class FlipPDA : Command, IParameters
{
    public enum Direction
    {
        Horizontal,
        Vertical,
    }
    public IReadOnlyList<Parameter> Parameters => [
        new(new("direction", "Direction", "Direction to flip"), typeof(Direction)),
    ];
    private static bool _init;

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Deny("Requires a loaded game.");

        PDA pda = Player.main.GetPDA();
        if (!pda) return CommonResults.Error("Could not get PDA."); // should never ever ever happen

        NamedArgs args = ctx.Input.GetNamedArguments();
        if (!args.TryGetValue("direction", out Direction dir)
            || dir is < Direction.Horizontal or > Direction.Vertical)
        {
            return CommonResults.Error("Invalid direction");
        }

        Vector3 pdaScale = pda.transform.localScale;
        Vector3 uiScale = pda.ui.transform.localScale;

        switch (dir)
        {
            case Direction.Horizontal:
                pdaScale.x = -pdaScale.x;
                pda.transform.localPosition = pda.transform.localPosition with
                {
                    x = pdaScale.x > 0 ? 0 : -0.35f,
                };
                uiScale.x = -uiScale.x;
                break;
            case Direction.Vertical:
                pdaScale.y = -pdaScale.y;
                uiScale.y = -uiScale.y;
                break;
        }

        pda.transform.localScale = pdaScale;
        pda.ui.transform.localScale = uiScale;
        if (!_init)
        {
            // if we flip vertically, all text blocks will face outwards and won't render their backface unless we do this disgusting thing
            foreach (TextMeshProUGUI text in pda.ui.GetComponentsInChildren<TextMeshProUGUI>(true)) // hundreds, yikes
            {
                // text.enableCulling is supposed to instantiate the material (yikes) but doesn't (y i k e s)
                // if we don't instantiate, HUD resource bars (HP, O2, etc) break bc they're double sided and do need to cull
                /// <see cref="TextMeshProUGUI.materialForRendering"/> getter
                text.enableCulling = true;
                // set twice because:
                // - there's a check that skips assignment if it's already set to this value
                /// <see cref="TextMeshProUGUI.enableCulling"/> setter
                // - the value it checks against is instanced (womp womp)
                /// <see cref="TextMeshProUGUI.m_isCullingEnabled"/>
                // and the material is... not instanced
                text.enableCulling = false;
            }

            foreach (TextMeshProUGUI barText in uGUI.main.barsPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                barText.fontMaterial = barText.fontMaterial; // instantiate material
                barText.enableCulling = false; // see above
                barText.enableCulling = true;
            }

            _init = true;
        }

        return CommonResults.OK();
    }
}
