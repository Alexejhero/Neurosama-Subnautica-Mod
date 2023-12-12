using SCHIZO.Attributes;
using SCHIZO.Items.Data;
using SCHIZO.Registering;
using SCHIZO.Spawns;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/PDA Journal")]
    [DeclareBoxGroup("Subnautica")]
    [DeclareBoxGroup("Below Zero")]
    public sealed partial class PDAJournal : ModRegistryItem
    {
        [Careful]
        public string key;
        [LabelText("PDA Hover Text")]
        public string pdaHandTargetText;
        [LabelText("PDA Hover Subtext")]
        public string pdaHandTargetSubtext;
        public PDAEncyclopediaInfo encyData;
        [InfoBox("Make sure the number of subtitle lines matches the number of audio clips in the event!", TriMessageType.Warning)]
        public Subtitles.SubtitlesData subtitles;
        
        [GroupNext("Subnautica")]
        public bool spawnInSN;
        [Careful, EnableIf(nameof(spawnInSN))]
        public SpawnLocation spawnLocationSN;
        [GroupNext("Below Zero")]
        public bool spawnInBZ;
        [Careful, EnableIf(nameof(spawnInBZ))]
        public SpawnLocation spawnLocationBZ;
    }
}
