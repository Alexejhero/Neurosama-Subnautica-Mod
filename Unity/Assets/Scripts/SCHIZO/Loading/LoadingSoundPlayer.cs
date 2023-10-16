using NaughtyAttributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Loading
{
    public sealed partial class LoadingSoundPlayer : MonoBehaviour
    {
        [Required] public BaseSoundCollection sounds;
    }
}
