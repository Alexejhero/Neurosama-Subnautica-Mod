using System.Collections.Generic;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;

namespace SCHIZO.Sounds;

partial class ItemSounds
{
    private static readonly Dictionary<TechType, string> _eatSounds = [];
    private Pickupable Pickupable => (Pickupable) pickupable;

    public static bool TryGet(TechType techType, out string eatSounds)
    {
        return _eatSounds.TryGetValue(techType, out eatSounds);
    }

    public void Register(TechType techType)
    {
        if (!_eatSounds.ContainsKey(techType))
            _eatSounds[techType] = eat;
    }

    public void Start()
    {
        if (!pickupable)
        {
            LOGGER.LogError($"{name} has no pickupable, self-destructing");
            Destroy(this);
            return;
        }

        Pickupable.pickedUpEvent.AddHandler(this, OnPickup);
        if (tool)
        {
            tool.drawSound = tool.drawSoundUnderwater = AudioUtils.GetFmodAsset(draw);
            tool.holsterSoundAboveWater = tool.holsterSoundUnderwater = AudioUtils.GetFmodAsset(holster);
        }
    }

    public void OnDestroy()
    {
        Pickupable.pickedUpEvent.RemoveHandler(this, OnPickup);
    }

    public void OnPickup(Pickupable _)
    {
        FMODHelpers.PlayPath2D(pickup);
    }
    // called through SendMessage (i love unity)
    public void OnDrop()
    {
        FMODHelpers.StopAllInstances(holster);
        emitter.PlayPath(drop);

        GetComponentsInChildren<InventoryAmbientSoundPlayer>().ForEach(p => p.Stop());
    }

    public static void OnCook(TechType techType, float delay = 0)
    {
        string cookSounds = _eatSounds.GetOrDefault(techType, null);
        FMODHelpers.PlayPath2D(cookSounds, delay);
    }

    public void OnEat()
    {
        FMODHelpers.StopAllInstances(holster);
        FMODHelpers.PlayPath2D(eat);
    }
    // SendMessage
    public void OnKill()
    {
        FMODHelpers.PlayPath2D(playerDeath, 0.15f);
    }
}
