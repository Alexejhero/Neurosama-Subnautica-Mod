using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

internal enum TrollNoise
{
    [EnumMember(Value = "Someone tell Vedal")]
    SomeoneTellVedal,
    [EnumMember(Value = "Leviathan Roar")]
    Leviathan,
    [EnumMember(Value = "Discord Disconnect")]
    DiscordDisconnect,
    [EnumMember(Value = "Discord Join")]
    DiscordJoin,
    [EnumMember(Value = "USB Disconnect")]
    UsbDisconnect,
    [EnumMember(Value = "USB Connect")]
    UsbConnect,
    Grindr,
    [EnumMember(Value = "Filian Scream")]
    FilianScream,
    [EnumMember(Value = "Program in C")]
    ProgramInC,
    [EnumMember(Value = "Stereo Knock")]
    StereoKnock,
}
