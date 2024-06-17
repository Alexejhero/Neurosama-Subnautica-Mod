using System;
using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems;

[AttributeUsage(AttributeTargets.Class)]
internal class RedeemAttribute : CommandAttribute
{
    /// <summary>
    /// Whether the redeem will show a "User redeemed X" message on-screen.<br/>
    /// For redeems that already display messages (or big secret funnies), this should be <see langword="false"/>.
    /// </summary>
    public bool Announce { get; set; } = true;
    public bool Export { get; set; } = true;
}
