using System;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Loading
{
    [DeclareBoxGroup("Sprite sheet")]
    [DeclareBoxGroup("Animations")]
    [DeclareBoxGroup("Other properties")]
    public sealed partial class BZErmsharkLoadingIcon : MonoBehaviour
    {
        [Required, GroupNext("Sprite sheet")]
        public Texture2D texture;
        public int rows;
        public int columns;
        
        [GroupNext("Animations")]
        public FrameAnimation idle;
        public FrameAnimation moving;
        public FrameAnimation stopping;

        [GroupNext("Other properties")]
        [InfoBox("Speed modifier evaluated on (time / duration). Applies only to the Moving animation.")]
        public AnimationCurve movingLoopSpeedCurve;
        public AnimationCurve idleToMovingSpeedCurve;
        [Serializable]
        public partial struct FrameAnimation
        {
            public int from;
            public int to;
            public float framerate;
            public readonly int frameCount => to - from + 1;
        }
    }
}
