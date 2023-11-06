using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum EcoTargetType_All
    {
        [Game(GameX.Both)] None = 0,
        [Game(GameX.Both)] Shiny = 1,
        [Game(GameX.Both)] DeadMeat = 2,
        [Game(GameX.Both)] Coral = 3,
        [Game(GameX.Both)] HeatArea = 4,
        [Game(GameX.Both)] Tech = 100, // 0x00000064
        [Game(GameX.Both)] Fragment = 150, // 0x00000096
        [Game(GameX.Both)] HoleFish = 1000, // 0x000003E8
        [Game(GameX.Both)] HoopFish = 1010, // 0x000003F2
        [Game(GameX.Both)] Peeper = 1020, // 0x000003FC
        [Game(GameX.Both)] Oculus = 1030, // 0x00000406
        [Game(GameX.Both)] SpadeFish = 1040, // 0x00000410
        [Game(GameX.Both)] CuteFish = 1050, // 0x0000041A
        [Game(GameX.Both)] LavaLarva = 1060, // 0x00000424
        [Game(GameX.Both)] Biter = 1070, // 0x0000042E
        [Game(GameX.Both)] Mushroom = 2000, // 0x000007D0
        [Game(GameX.Both)] HeatSource = 2005, // 0x000007D5
        [Game(GameX.Both)] SmallFish = 2010, // 0x000007DA
        [Game(GameX.Both)] MediumFish = 2020, // 0x000007E4
        [Game(Game.BelowZero)] FishSchool = 2025, // 0x000007E9
        [Game(GameX.Both)] Shark = 2030, // 0x000007EE
        [Game(GameX.Both)] Whale = 2040, // 0x000007F8
        [Game(GameX.Both)] Leviathan = 2045, // 0x000007FD
        [Game(GameX.Both)] Poison = 2050, // 0x00000802
        [Game(GameX.Both)] Trap = 2060, // 0x0000080C
        [Game(GameX.Both)] Cure = 2070, // 0x00000816
        [Game(GameX.Both)] CuredWarp = 2080, // 0x00000820
        [Game(GameX.Both)] SubDecoy = 2090, // 0x0000082A
    }
}
