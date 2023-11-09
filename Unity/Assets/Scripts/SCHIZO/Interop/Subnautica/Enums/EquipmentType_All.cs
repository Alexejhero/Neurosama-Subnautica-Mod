using SCHIZO.Registering;

namespace SCHIZO.Interop.Subnautica.Enums
{
    public enum EquipmentType_All
    {
        [Game(Game.Subnautica | Game.BelowZero)] None,
        [Game(Game.Subnautica | Game.BelowZero)] Hand,
        [Game(Game.Subnautica | Game.BelowZero)] Head,
        [Game(Game.Subnautica | Game.BelowZero)] Body,
        [Game(Game.Subnautica | Game.BelowZero)] Gloves,
        [Game(Game.Subnautica | Game.BelowZero)] Foots,
        [Game(Game.Subnautica | Game.BelowZero)] Tank,
        [Game(Game.Subnautica | Game.BelowZero)] Chip,
        [Game(Game.Subnautica)] CyclopsModule,
        [Game(Game.Subnautica | Game.BelowZero)] VehicleModule,
        [Game(Game.Subnautica | Game.BelowZero)] NuclearReactor,
        [Game(Game.Subnautica | Game.BelowZero)] BatteryCharger,
        [Game(Game.Subnautica | Game.BelowZero)] PowerCellCharger,
        [Game(Game.Subnautica)] SeamothModule,
        [Game(Game.Subnautica | Game.BelowZero)] ExosuitModule,
        [Game(Game.Subnautica | Game.BelowZero)] ExosuitArm,
        [Game(Game.Subnautica | Game.BelowZero)] DecoySlot,
        [Game(Game.BelowZero)] OldFood,
        [Game(Game.BelowZero)] HoverbikeModule,
        [Game(Game.BelowZero)] SeaTruckModule,
    }
}
