using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Misc;
[Redeem(
    Name = "redeem_midas",
    DisplayName = "Midas' Touch",
    Description = "And all that glitters is gold"
)]
public sealed class MidasTouch : Command, IParameters
{
    private static readonly HashSet<TechType> _turnedItems = [
        TechType.Salt,
        TechType.Quartz,
        TechType.ScrapMetal,
        TechType.Titanium,
        TechType.Copper,
        TechType.Lead,
        TechType.Silver,
        TechType.Gold, // funny
        TechType.Diamond,
        TechType.Lithium,
        TechType.Magnetite,
        TechType.Kyanite,
        TechType.Nickel,

        TechType.TwistyBridgesMushroomChunk,
        TechType.GenericRibbon,
        TechType.JeweledDiskPiece, // table coral
        TechType.GenericSpiralChunk,

        TechType.Bladderfish,
        TechType.ArcticPeeper,
        TechType.SpinnerFish,
        TechType.Boomerang,
        TechType.Brinewing,
        TechType.Hoopfish,
        TechType.Spinefish,
        TechType.PenguinBaby, // uh oh
        TechType.Rockgrub,
        TechType.NootFish,
        TechType.Symbiote,
        TechType.DiscusFish,
        TechType.FeatherFishRed,

        TechType.NutrientBlock,
        TechType.WaterPurificationTablet,
        TechType.BigFilteredWater,
        TechType.DisinfectedWater,
        TechType.FilteredWater,
    ];

    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Inventory.main) return CommonResults.Error("Requires a loaded game");
        InventoryItem[] items = Inventory.main.container.GetAllItems()
            .Where(i => _turnedItems.Contains(i.techType))
            .ToArray();
        if (items.Length == 0)
        {
            return CommonResults.Error("No items to turn");
        }
        foreach (InventoryItem item in items)
        {
            Inventory.main.container.RemoveItem(item.item, true);
        }
        CoroutineHost.StartCoroutine(InventoryConsoleCommands.ItemCmdSpawnAsync(items.Length, TechType.Gold));
        return CommonResults.OK();
    }
}
