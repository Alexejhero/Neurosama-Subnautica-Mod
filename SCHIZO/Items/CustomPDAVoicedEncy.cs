using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using SCHIZO.Helpers;

namespace SCHIZO.Items;

[HarmonyPatch]
partial class CustomPDAVoicedEncy
{
    protected override void Register()
    {
        encyData.Register(key);
        Subtitles.SubtitlesHandler.RegisterMetadata(subtitles, encyData.description.text);
        JournalPDAPrefab.Register(this);
    }

    private sealed class JournalPDAPrefab
    {
        private const string BZ_JOURNAL_PDA_CLASSID = "1c7d135c-79f1-4b7a-9528-20c0570df824";
        private static string CloneTargetClassId => RetargetHelpers.Pick("TODO", BZ_JOURNAL_PDA_CLASSID);
        internal static Dictionary<string, JournalPDAPrefab> Prefabs = [];

        public CustomPrefab NautilusPrefab { get; }
        public CloneTemplate CloneTemplate { get; }
        public PrefabInfo Info { get; }
        private JournalPDAPrefab(CustomPDAVoicedEncy ency)
        {
            Info = new PrefabInfo
            {
                ClassID = ClassIdFor(ency.key),
                PrefabFileName = $"{nameof(JournalPDAPrefab)}_{ency.key}"
                // no techtype
            };
            CloneTemplate = new CloneTemplate(Info, CloneTargetClassId)
            {
                ModifyPrefab = prefab =>
                {
                    prefab.name = $"PDA_{ency.key}";
                    StoryHandTarget handTarget = prefab.GetComponent<StoryHandTarget>();
                    if (!string.IsNullOrEmpty(ency.pdaHandTargetText))
                        handTarget.primaryTooltip = ency.pdaHandTargetText;
                    if (!string.IsNullOrEmpty(ency.pdaHandTargetSubtext))
                        handTarget.secondaryTooltip = ency.pdaHandTargetSubtext;
                    handTarget.goal.key = ency.key;
                }
            };

            NautilusPrefab = new CustomPrefab() { Info = Info };
            NautilusPrefab.SetGameObject(CloneTemplate);
            NautilusPrefab.Register();
        }

        public static void Register(CustomPDAVoicedEncy ency)
        {
            JournalPDAPrefab prefab = new(ency);
            Prefabs[ency.key] = prefab;
            ency.spawns.Where(s => s.game == GAME)
                .SelectMany(spawn => spawn.locations)
                .ForEach(loc =>
                {
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(prefab.Info.ClassID, loc.position, loc.rotation));
                });
        }

        private static string ClassIdFor(string key)
        {
            int modNameHash = nameof(SCHIZO).GetHashCode();
            int entTypeHash = nameof(JournalPDAPrefab).GetHashCode();
            int targetHash = CloneTargetClassId.GetHashCode();
            int keyHash = key.GetHashCode();

            return $"{modNameHash:x08}{entTypeHash:x08}{targetHash:x08}{keyHash:x08}";
        }
    }
}
