using System.Collections.Generic;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Sounds;

partial class ItemSounds
{
    private static readonly Dictionary<TechType, string> _cookSounds = [];
    private Pickupable Pickupable => (Pickupable) pickupable;

    public void Awake()
    {
        TechType techType = CraftData.GetTechType(gameObject);
        if (!_cookSounds.ContainsKey(techType))
            _cookSounds[techType] = cook;
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
            PlayerTool Tool = (PlayerTool)tool;
            Tool.drawSound = Tool.drawSoundUnderwater = FMODHelpers.GetFmodAsset(draw);
            Tool.holsterSoundAboveWater = Tool.holsterSoundUnderwater = FMODHelpers.GetFmodAsset(holster);
        }
    }

    public void OnDestroy()
    {
        Pickupable.pickedUpEvent.RemoveHandler(this, OnPickup);
    }

    public void OnPickup(Pickupable _)
    {
        FMODHelpers.PlayPath2D(pickup);
        WorldAmbientSoundPlayer player = GetComponent<WorldAmbientSoundPlayer>();
        if (player) player.Stop();
    }
    // called through SendMessage (i love unity)
    public void OnDrop()
    {
        FMODHelpers.StopAllInstances(holster);
        foreach (InventoryAmbientSoundPlayer p in GetComponentsInChildren<InventoryAmbientSoundPlayer>())
        {
            p.Stop();
        }
        emitter.PlayPath(drop);
    }

    public static void OnCook(Crafter crafter, TechType techType, float delay = 0)
    {
        string cookSounds = _cookSounds.GetOrDefault(techType, null);
        Transform soundOrigin = crafter.transform;
        if (crafter is Fabricator fab)
            soundOrigin = fab.soundOrigin;
        FMODHelpers.PlayPath3DAttached(cookSounds, soundOrigin, delay);
    }

    public void OnEat()
    {
        // holster sound will terminate on its own since the object will be destroyed
        FMODHelpers.PlayPath2D(eat);
    }
}
