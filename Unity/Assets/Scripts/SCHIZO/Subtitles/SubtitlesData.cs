using System;
using System.Collections.Generic;
using SCHIZO.Attributes;
using SCHIZO.Registering;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Subtitles
{
    [CreateAssetMenu(menuName = "SCHIZO/Subtitles/Subtitles Data")]
    public sealed partial class SubtitlesData : ModRegistryItem
    {
        [Careful]
        public string key;
        [TableList]
        public List<SubtitleLine> lines;

        [Serializable]
        public sealed partial class SubtitleLine
        {
            public string key;
            public ActorData actor;
            [TextArea]
            public string text;
        }
    }

}
