using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum EcoTargetType_All
    {
        [Game(Game.Subnautica | Game.BelowZero)] None = 0,
        [Game(Game.Subnautica | Game.BelowZero)] Shiny = 1,
        [Game(Game.Subnautica | Game.BelowZero)] DeadMeat = 2,
        [Game(Game.Subnautica | Game.BelowZero)] Coral = 3,
        [Game(Game.Subnautica | Game.BelowZero)] HeatArea = 4,
        [Game(Game.Subnautica | Game.BelowZero)] Tech = 100, // 0x00000064
        [Game(Game.Subnautica | Game.BelowZero)] Fragment = 150, // 0x00000096
        [Game(Game.Subnautica | Game.BelowZero)] HoleFish = 1000, // 0x000003E8
        [Game(Game.Subnautica | Game.BelowZero)] HoopFish = 1010, // 0x000003F2
        [Game(Game.Subnautica | Game.BelowZero)] Peeper = 1020, // 0x000003FC
        [Game(Game.Subnautica | Game.BelowZero)] Oculus = 1030, // 0x00000406
        [Game(Game.Subnautica | Game.BelowZero)] SpadeFish = 1040, // 0x00000410
        [Game(Game.Subnautica | Game.BelowZero)] CuteFish = 1050, // 0x0000041A
        [Game(Game.Subnautica | Game.BelowZero)] LavaLarva = 1060, // 0x00000424
        [Game(Game.Subnautica | Game.BelowZero)] Biter = 1070, // 0x0000042E
        [Game(Game.Subnautica | Game.BelowZero)] Mushroom = 2000, // 0x000007D0
        [Game(Game.Subnautica | Game.BelowZero)] HeatSource = 2005, // 0x000007D5
        [Game(Game.Subnautica | Game.BelowZero)] SmallFish = 2010, // 0x000007DA
        [Game(Game.Subnautica | Game.BelowZero)] MediumFish = 2020, // 0x000007E4
        [Game(Game.BelowZero)] FishSchool = 2025, // 0x000007E9
        [Game(Game.Subnautica | Game.BelowZero)] Shark = 2030, // 0x000007EE
        [Game(Game.Subnautica | Game.BelowZero)] Whale = 2040, // 0x000007F8
        [Game(Game.Subnautica | Game.BelowZero)] Leviathan = 2045, // 0x000007FD
        [Game(Game.Subnautica | Game.BelowZero)] Poison = 2050, // 0x00000802
        [Game(Game.Subnautica | Game.BelowZero)] Trap = 2060, // 0x0000080C
        [Game(Game.Subnautica | Game.BelowZero)] Cure = 2070, // 0x00000816
        [Game(Game.Subnautica | Game.BelowZero)] CuredWarp = 2080, // 0x00000820
        [Game(Game.Subnautica | Game.BelowZero)] SubDecoy = 2090, // 0x0000082A
    }
}
