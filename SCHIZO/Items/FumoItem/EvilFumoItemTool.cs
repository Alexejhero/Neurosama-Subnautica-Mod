namespace SCHIZO.Items.FumoItem;

partial class EvilFumoItemTool
{
    public Knife stolenKnife;

    protected override void ApplyAltEffect(bool active)
    {
        if (!stealKnife) return;
        LOGGER.LogWarning(active);
        if (active)
        {
            float dmg = damageOnPoke;
            if (TryFindKnife(out Knife knife)
                && Inventory.main.InternalDropItem(knife.pickupable))
            {
                stolenKnife = knife;
                YoinkKnife();
                damageOnPoke *= 2;
            }
            usingPlayer.liveMixin.TakeDamage(dmg);
        }
        else
        {
            if (!ReturnKnife())
                LOGGER.LogError("Could not return stolen knife");
        }
    }

    private bool TryFindKnife(out Knife knife)
    {
        knife = default;

        QuickSlots slots = Inventory.main.quickSlots;
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = slots.GetSlotItem(i);
            if (item is null || !item.item)
                continue;
            Knife knifeMaybe = item.item.GetComponent<Knife>();
            if (!knifeMaybe) continue;
            knife = knifeMaybe;
            return true;
        }
        return false;
    }

    private void YoinkKnife()
    {
        PlugIntoSocket(stolenKnife, transform);
    }

    private bool ReturnKnife()
    {
        if (!stolenKnife) return true;
        DropKnife();

        if (!Inventory.main.Pickup(stolenKnife.pickupable)) return false;
        return true;
    }

    private void DropKnife()
    {
        stolenKnife.transform.parent = null;
    }
}
