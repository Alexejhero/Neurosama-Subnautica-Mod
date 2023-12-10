using System.Collections.Generic;
using SCHIZO.Items.Data;
using SCHIZO.Registering;
using SCHIZO.Spawns;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/PDA Journal Entry")]
    public sealed partial class CustomPDAVoicedEncy : ModRegistryItem
    {
        public string key;
        [LabelText("PDA Hover Text")]
        public string pdaHandTargetText;
        [LabelText("PDA Hover Subtext")]
        public string pdaHandTargetSubtext;
        public PDAEncyclopediaInfo encyData;
        [InfoBox("Make sure the number of subtitle lines matches the number of audio clips in the event!", TriMessageType.Warning)]
        public Subtitles.SubtitlesData subtitles;
        public List<SpawnInfo> spawns;
    }
}
