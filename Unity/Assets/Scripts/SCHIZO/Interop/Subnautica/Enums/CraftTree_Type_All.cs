using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum CraftTree_Type_All
    {
        [Game(Game.Subnautica | Game.BelowZero)] None,
        [Game(Game.Subnautica | Game.BelowZero)] Fabricator,
        [Game(Game.Subnautica | Game.BelowZero)] Constructor,
        [Game(Game.Subnautica | Game.BelowZero)] Workbench,
        [Game(/*Game.Subnautica | Game.BelowZero*/)] Unused1,
        [Game(/*Game.Subnautica | Game.BelowZero*/)] Unused2,
        [Game(Game.Subnautica | Game.BelowZero)] SeamothUpgrades,
        [Game(Game.Subnautica | Game.BelowZero)] MapRoom,
        [Game(/*Game.Subnautica | Game.BelowZero*/)] Centrifuge,
        [Game(Game.Subnautica /*| Game.BelowZero*/)] CyclopsFabricator,
        [Game(/*Game.Subnautica | Game.BelowZero*/)] Rocket,
        [Game(Game.BelowZero)] SeaTruckFabricator,
        [Game(/*Game.BelowZero*/)] PrecursorPartOrgans,
        [Game(/*Game.BelowZero*/)] PrecursorPartTissue,
        [Game(/*Game.BelowZero*/)] PrecursorPartSkeleton,
    }
}
