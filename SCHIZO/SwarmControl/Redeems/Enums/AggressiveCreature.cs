using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

public enum AggressiveCreature
{
    Ermshark,
    [EnumMember(Value = "Lily Paddler")]
    LilyPaddler,
    Cryptosuchus,
    [EnumMember(Value = "Squid Shark")]
    SquidShark,
}
