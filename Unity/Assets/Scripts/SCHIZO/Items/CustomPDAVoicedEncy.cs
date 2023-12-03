using System.Collections.Generic;
using SCHIZO.Items.Data;
using SCHIZO.Registering;
using SCHIZO.Sounds.Collections;
using SCHIZO.Spawns;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Custom PDA log")]
    public sealed partial class CustomPDAVoicedEncy : ModRegistryItem
    {
        public string key;
        public PDAEncyclopediaInfo encyData;
        [InfoBox("Make sure the number of audio clips matches the number of subtitle lines!", TriMessageType.Warning)]
        public SoundCollection logVO;
        public Subtitles.SubtitlesData subtitles;
        public List<SpawnInfo> spawns;
    }
}
