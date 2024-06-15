using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

public enum AggressiveCreature
{
    Ermshark,
    Brinewing,
    Cryptosuchus,
    [EnumMember(Value = "Squid Shark")]
    SquidShark,
}
