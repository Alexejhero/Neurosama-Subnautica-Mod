using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

internal enum NormalNoise
{
    [EnumMember(Value = "Ermfish Ambient")]
    ErmfishAmbient,
    [EnumMember(Value = "Ermfish Cook")]
    ErmfishCook,
    [EnumMember(Value = "Ermfish Eat")]
    ErmfishEat,
    [EnumMember(Value = "Anneel Ambient")]
    AnneelAmbient,
    [EnumMember(Value = "Anneel Hurt")]
    AnneelHurt,
    [EnumMember(Value = "Tutel Ambient")]
    TutelAmbient,
    [EnumMember(Value = "Tutel Cook")]
    TutelCook,
    [EnumMember(Value = "Tutel Eat")]
    TutelEat,
    [EnumMember(Value = "Ermshark Ambient")]
    ErmsharkAmbient,
    [EnumMember(Value = "Ermshark Attack")]
    ErmsharkAttack,
    [EnumMember(Value = "Ermshark Hurt")]
    ErmsharkHurt
}
