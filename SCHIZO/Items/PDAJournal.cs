using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using SCHIZO.Helpers;

namespace SCHIZO.Items;

partial class PDAJournal
{
    protected override void Register()
    {
        encyData.Register(key);
        Subtitles.SubtitlesHandler.RegisterMetadata(subtitles, encyData.description.text);
        PDAJournalPrefab.Register(this);
    }

    private sealed class PDAJournalPrefab
    {
        private const string SN_JOURNAL_PDA_CLASSID = "c060e7fd-f4d2-4bef-8862-f31420d60ba0";
        private const string BZ_JOURNAL_PDA_CLASSID = "1c7d135c-79f1-4b7a-9528-20c0570df824";
        private static string CloneTargetClassId => RetargetHelpers.Pick(SN_JOURNAL_PDA_CLASSID, BZ_JOURNAL_PDA_CLASSID);
        internal static Dictionary<string, PDAJournalPrefab> Prefabs = [];

        public CustomPrefab NautilusPrefab { get; }
        private PDAJournalPrefab(PDAJournal journal)
        {
            string prefabName = $"{nameof(PDAJournal)}_{journal.key}";

            NautilusPrefab = new CustomPrefab(prefabName, null, null);

            bool doSpawn = RetargetHelpers.Pick(journal.spawnInSN, journal.spawnInBZ);
            if (doSpawn)
            {
                Spawns.SpawnLocation ourLoc = RetargetHelpers.Pick(journal.spawnLocationSN, journal.spawnLocationBZ);
                SpawnLocation loc = new(ourLoc.position, ourLoc.rotation);
                NautilusPrefab.SetSpawns(loc);
            }
            NautilusPrefab.SetGameObject(new CloneTemplate(NautilusPrefab.Info, CloneTargetClassId)
            {
                ModifyPrefab = prefab =>
                {
                    StoryHandTarget handTarget = prefab.GetComponent<StoryHandTarget>();
                    if (!string.IsNullOrEmpty(journal.pdaHandTargetText))
                        handTarget.primaryTooltip = journal.pdaHandTargetText;
                    if (!string.IsNullOrEmpty(journal.pdaHandTargetSubtext))
                        handTarget.secondaryTooltip = journal.pdaHandTargetSubtext;
                    handTarget.goal.key = journal.key;
                }
            });
            NautilusPrefab.Register();
        }

        public static void Register(PDAJournal journal)
        {
            if (Prefabs.ContainsKey(journal.key))
            {
                LOGGER.LogWarning($"Dupe PDAJournalPrefab {journal.key}");
                return;
            }
            Prefabs[journal.key] = new PDAJournalPrefab(journal);
        }
    }
}
