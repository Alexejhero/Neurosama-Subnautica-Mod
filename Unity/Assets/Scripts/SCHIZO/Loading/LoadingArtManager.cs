using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Loading
{
    public sealed partial class LoadingArtManager : MonoBehaviour
    {
        [SerializeField, Required] private LoadingBackgroundCollection loadingScreens;
    }
}
