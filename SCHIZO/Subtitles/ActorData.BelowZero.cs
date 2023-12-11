using System;
using HarmonyLib;
using Nautilus.Handlers;

namespace SCHIZO.Subtitles;

[HarmonyPatch]
partial class ActorData
{
    protected override void Register()
    {
        if (!EnumHandler.TryAddEntry<Actor>(identifier, PLUGIN_ASSEMBLY, out _))
        {
            LOGGER.LogWarning($"Name conflict - Actor '{identifier}' is already registered! Skipping");
            return;
        }

        LanguageHandler.SetLanguageLine(identifier, displayName);
        SpriteHandler.RegisterSprite(SpriteManager.Group.Portraits, identifier, sprite);
    }

    public static implicit operator Actor(ActorData data)
        => EnumHandler.TryGetValue(data.identifier, out Actor value) ? value
            : throw new InvalidOperationException($"Actor '{data.identifier}' is not registered");

    [HarmonyPatch(typeof(uGUI_TalkingHead), nameof(uGUI_TalkingHead.GetSkin))]
    [HarmonyPrefix]
    private static bool GetCustomSkin(Actor actor, out TalkingHeadSkin __result)
    {
        __result = default;

        if (actor > Actor.Unknown) // modded
        {
            __result = TalkingHeadSkin.Default;
            return false;
        }
        return true;
    }
}
