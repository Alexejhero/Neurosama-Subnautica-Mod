using System;
using System.Collections.Generic;
using SCHIZO.Registering;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Subtitles
{
    [CreateAssetMenu(menuName = "SCHIZO/Subtitles/Subtitles Data")]
    public sealed partial class SubtitlesData : ModRegistryItem
    {
        [Tooltip("Leave blank if these subtitles are for an ency PDA log")]
        public string key;
        [TableList]
        public List<SubtitleLine> lines;

        [Serializable]
        public sealed partial class SubtitleLine
        {
            [Tooltip("Name of the \"instrument\" (individual audio clip) in the FMOD event")]
            public string key;
            public ActorData actor;
            [TextArea]
            public string text;
        }
    }
}
