using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public partial class SneakyFumoSpawner : MonoBehaviour
    {
        public float minAwayTime = 60f;
        public float minAwayDistance = 100f;
        public ItemData spawnData;
    }
}