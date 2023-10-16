using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Loading
{
    public sealed partial class FumoLoadingIcon : MonoBehaviour
    {
        [SerializeField, Required] private Texture2D texture;
    }
}
