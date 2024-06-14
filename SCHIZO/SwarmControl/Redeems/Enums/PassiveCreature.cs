using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

public enum PassiveCreature
{
    Ermfish,
    Anneel,
    Tutel,
    [EnumMember(Value = "Arctic Peeper")]
    ArcticPeeper,
    Bladderfish,
    Boomerang,
    [EnumMember(Value = "Spinner Fish")]
    SpinnerFish,
}
