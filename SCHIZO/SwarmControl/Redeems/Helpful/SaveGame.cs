using System.Collections.Generic;
using Nautilus.Utility;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Helpful;

[Redeem(
    Name = "redeem_save",
    DisplayName = "Save Game",
    Description = "Please don't spam this one or we will disable it"
)]
internal class SaveGame : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    private float _lastSave;
    public SaveGame()
    {
        SaveUtils.RegisterOnFinishLoadingEvent(() => _lastSave = PDA.time);
        SaveUtils.RegisterOnSaveEvent(() => _lastSave = PDA.time);
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main)
            return CommonResults.Error("Requires a loaded game");
        if (PDA.time - _lastSave >= 60f)
        {
            CoroutineHost.StartCoroutine(IngameMenu.main.SaveGameAsync());
            _lastSave = PDA.time;
            return CommonResults.OK();
        }
        else // yoink money from spammers
        {
            return "Last save was less than a minute ago. Rest assured the game is saved.";
        }
    }
}
