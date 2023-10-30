using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum EcoTargetType_All
    {
        [Game(Game.Both)] None = 0,
        [Game(Game.Both)] Shiny = 1,
        [Game(Game.Both)] DeadMeat = 2,
        [Game(Game.Both)] Coral = 3,
        [Game(Game.Both)] HeatArea = 4,
        [Game(Game.Both)] Tech = 100, // 0x00000064
        [Game(Game.Both)] Fragment = 150, // 0x00000096
        [Game(Game.Both)] HoleFish = 1000, // 0x000003E8
        [Game(Game.Both)] HoopFish = 1010, // 0x000003F2
        [Game(Game.Both)] Peeper = 1020, // 0x000003FC
        [Game(Game.Both)] Oculus = 1030, // 0x00000406
        [Game(Game.Both)] SpadeFish = 1040, // 0x00000410
        [Game(Game.Both)] CuteFish = 1050, // 0x0000041A
        [Game(Game.Both)] LavaLarva = 1060, // 0x00000424
        [Game(Game.Both)] Biter = 1070, // 0x0000042E
        [Game(Game.Both)] Mushroom = 2000, // 0x000007D0
        [Game(Game.Both)] HeatSource = 2005, // 0x000007D5
        [Game(Game.Both)] SmallFish = 2010, // 0x000007DA
        [Game(Game.Both)] MediumFish = 2020, // 0x000007E4
        [Game(Game.BelowZero)] FishSchool = 2025, // 0x000007E9
        [Game(Game.Both)] Shark = 2030, // 0x000007EE
        [Game(Game.Both)] Whale = 2040, // 0x000007F8
        [Game(Game.Both)] Leviathan = 2045, // 0x000007FD
        [Game(Game.Both)] Poison = 2050, // 0x00000802
        [Game(Game.Both)] Trap = 2060, // 0x0000080C
        [Game(Game.Both)] Cure = 2070, // 0x00000816
        [Game(Game.Both)] CuredWarp = 2080, // 0x00000820
        [Game(Game.Both)] SubDecoy = 2090, // 0x0000082A
    }
}
