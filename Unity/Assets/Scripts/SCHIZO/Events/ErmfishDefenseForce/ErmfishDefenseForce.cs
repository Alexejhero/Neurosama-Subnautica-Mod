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
        [Tooltip("Aggro threshold after which to start spawning")]
        public float startAggroThreshold;
        public float startCooldown = 1800f;

        [GroupNext("aggro")]
        public float pickUpAggro;
        public float dropAggro;
        public float attackAggro;
        public float cookAggro;
        public float eatAggro;
        [InfoBox("This will not reduce aggro below zero")]
        public float killedByDefenderAggro;
        [InfoBox("todo ermcon scoring system"), ReadOnly]
        public Vector2Int ermconScoreAggro = new(-10, 50);
        [UnGroupNext]

        [LabelText("Defense Force")]
        public Defender[] defenders;

        [Serializable]
        public partial class Defender
        {
            public string name;
            [SerializeField]
            private ItemData _defender;
            public string ClassId => _defender.classId;
            [Tooltip("Cost to spawn each of this type of defender")]
            public float aggroCost;
            [Min(0), Tooltip("Maximum number of these that can be spawned at once in a group")]
            public int maxGroupSize;
            [Min(0), Tooltip("Relative to all the other defenders (e.g. if this has 2 and someone else has 5, this will spawn 2/7th of the time)")]
            public int spawnWeight;
        }
    }
}
