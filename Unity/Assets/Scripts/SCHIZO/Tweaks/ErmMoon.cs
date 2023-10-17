using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Tweaks
{
    public sealed partial class ErmMoon : MonoBehaviour
    {
        [SerializeField, Required, ValidateInput(nameof(ValidateErmText), "Erm texture must be readable!"), UsedImplicitly]
        private Texture2D ermTexture;

        private bool ValidateErmText(Texture2D input) => !input || input.isReadable;
    }
}
