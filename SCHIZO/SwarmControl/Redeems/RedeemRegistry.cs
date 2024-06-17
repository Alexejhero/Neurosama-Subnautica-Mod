using System.Collections.Generic;
using SCHIZO.Commands.Base;

namespace SCHIZO.SwarmControl.Redeems;
internal static class RedeemRegistry
{
    internal static readonly Dictionary<string, Redeem> Redeems = [];

    public static void Register(RedeemAttribute attr, Command command)
    {
        Redeems[attr.Name] = new Redeem(attr, command);
    }
}
