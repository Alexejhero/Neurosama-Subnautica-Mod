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
        [Careful, Required]
        public string key;
        [LabelText("PDA Hover Text")]
        public string pdaHandTargetText;
        [LabelText("PDA Hover Subtext")]
        public string pdaHandTargetSubtext;
        [Required]
        public PDAEncyclopediaInfo encyData;
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
