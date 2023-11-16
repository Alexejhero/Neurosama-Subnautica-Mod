using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Tweaks
{
    public sealed partial class ErmMoon : MonoBehaviour
    {
        [SerializeField, ValidateInput(nameof(ValidateErmText)), UsedImplicitly]
        private Texture2D ermTexture;

        private TriValidationResult ValidateErmText()
        {
            if (!ermTexture) return TriValidationResult.Error("Texture is required");
            if (!ermTexture.isReadable) return TriValidationResult.Error("Texture must be readable");
            return TriValidationResult.Valid;
        }
    }
}
