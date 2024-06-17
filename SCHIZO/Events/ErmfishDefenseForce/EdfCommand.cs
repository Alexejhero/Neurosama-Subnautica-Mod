using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Events.ErmfishDefenseForce;

[Command(Name = "edf",
    DisplayName = "Ermfish Defense Force",
    Description = "Commands for EDF (Ermfish Defense Force).\nEDF is an event that will spawn ermsharks when the player is mean to ermfish.",
    RegisterConsoleCommand = true)]
public class EdfCommand : CompositeCommand
{
    [SubCommand(DisplayName = "Check/set aggro", Description = "Leave value default to check aggro")]
    public string Aggro(float? value = null)
    {
        if (value is null)
            return ErmfishDefenseForce.instance.CurrentAggro.ToString();
        ErmfishDefenseForce.instance.SetAggro(value.Value, nameof(EdfCommand));
        return null;
    }

    [SubCommand(DisplayName = "Reset", Description = "Resets event so spawns can trigger again immediately")]
    public static void Reset()
    {
        ErmfishDefenseForce.instance.Reset();
    }
}
