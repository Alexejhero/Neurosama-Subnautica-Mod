using System;
using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems;

[AttributeUsage(AttributeTargets.Class)]
internal class RedeemAttribute : CommandAttribute
{
    public AnnounceType Announce { get; set; }
}
