using System;
using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems;

[AttributeUsage(AttributeTargets.Class)]
internal class RedeemAttribute : CommandAttribute
{
    public enum AnnounceType
    {
        DefaultAnnounce,
        DefaultSilent,
        Ugc // always announce
    }
    public AnnounceType DefaultAnnounce { get; }
}
