using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Helpers;

public static class InventoryExtensions
{
    public static IEnumerable<InventoryItem> GetAllItems(this ItemsContainer container)
    {
        return container.GetItemTypes().SelectMany(container.GetItems);
    }
}
