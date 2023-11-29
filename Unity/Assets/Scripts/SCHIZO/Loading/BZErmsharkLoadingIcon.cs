using System;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Loading
{
    [DeclareBoxGroup("Sprite sheet")]
    [DeclareBoxGroup("Animations")]
    public sealed partial class BZErmsharkLoadingIcon : MonoBehaviour
    {
        [Required, GroupNext("Sprite sheet")]
        public Texture2D texture;
        public int rows;
        public int columns;
        
        [GroupNext("Animations")]
        public Animation idle;
        public Animation moving;
        public Animation stopping;
        [Serializable]
        public partial struct Animation
        {
            public int from;
            public int to;
            public float framerate = 18;
            public readonly int frameCount => to - from + 1;

            public Animation() {} // required by compiler
        }
    }
}
