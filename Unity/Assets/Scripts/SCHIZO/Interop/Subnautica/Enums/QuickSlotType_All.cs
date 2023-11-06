using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum QuickSlotType_All
    {
        [Game(Game.Subnautica | Game.BelowZero)] None,
        [Game(Game.Subnautica | Game.BelowZero)] Passive,
        [Game(Game.Subnautica | Game.BelowZero)] Instant,
        [Game(Game.Subnautica | Game.BelowZero)] Selectable,
        [Game(Game.Subnautica | Game.BelowZero)] SelectableChargeable,
        [Game(Game.Subnautica | Game.BelowZero)] Chargeable,
        [Game(Game.Subnautica | Game.BelowZero)] Toggleable,
    }
}
