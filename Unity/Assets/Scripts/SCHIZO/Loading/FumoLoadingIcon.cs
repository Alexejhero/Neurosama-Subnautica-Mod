using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Loading
{
    public sealed partial class FumoLoadingIcon : MonoBehaviour
    {
        [SerializeField, Required, UsedImplicitly]
        private Texture2D texture;
    }
}
