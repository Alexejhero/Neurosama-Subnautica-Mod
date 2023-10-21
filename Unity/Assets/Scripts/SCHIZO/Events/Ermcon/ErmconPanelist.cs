using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    public partial class ErmconPanelist : MonoBehaviour
    {
        [Tooltip("How interested Ermcon aficionados are in this particular host.\nAffects their boredom growth and swim speed.")]
        [Range(0.1f, 10f)]
        public float entertainmentFactor = 1f;
        public float personalSpaceRadius = 3f;

        // exclusive ermcon sounds? scripted behaviours that affect the entertainment factor?
        // the possibilities are endless... time left on this earth, however, is not
    }
}
