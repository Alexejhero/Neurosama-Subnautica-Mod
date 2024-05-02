using System;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Events.ErmfishDefenseForce
{
    [DeclareFoldoutGroup("aggro", Title = "Aggro values")]
    public partial class ErmfishDefenseForce : GameEvent
    {
        [SerializeField]
        private ItemData[] protectedSpecies;
        [Tooltip("After spawning, aggro gain is disabled for this many seconds")]
        public float cooldown = 300f;

        [GroupNext("aggro")]
        [Min(0)]
        public float spawnThreshold = 150;
        [Min(0), Tooltip("First EDF per save")]
        public float firstTimeThreshold = 100;
        [LabelText("Decay (per second)")]
        public float decay = 2.5f;
        public float pickUpAggro = 25;
        public float attackAggro = 50;
        public float cookAggro = 10;
        public float eatAggro = 15;
        [InfoBox("todo ermcon scoring system"), ReadOnly]
        public Vector2Int ermconScoreAggro = new(-50, 10);
        [UnGroupNext]

        [LabelText("Defense Force")]
        public Defender[] defenders;
        public string[] messages;

        [Serializable]
        public partial class Defender
        {
            [SerializeField]
            private ItemData _defender;
            public string ClassId => _defender.classId;
            [Min(0), Tooltip("Maximum number of these that can be spawned at once in a group")]
            public int maxGroupSize;
            [Min(0), Tooltip("Relative to all the other defenders (e.g. if this has 2 and someone else has 5, this will spawn 2/7th of the time)")]
            public int spawnWeight;
        }
    }
}
