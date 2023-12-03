using System;
using SCHIZO.Attributes;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Subtitles
{
    [Serializable]
    [CreateAssetMenu(menuName = "SCHIZO/Subtitles/Actor Data")]
    public sealed partial class ActorData : ModRegistryItem
    {
        [Careful]
        public string identifier;
        public string displayName;
        public Sprite sprite;
    }
}
