using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    [CreateAssetMenu(menuName = "SCHIZO/VFX/VFXmaterial")]
    public class VFXMaterial : ScriptableObject
    {
        [Required]
        public string materialName;

        //TODO:
        //the idea was to make a copy while in the editor, so it can be changed for specific use case,
        //but when i make a new material in the editor, it does not make it into the game for some reason.
        [InlineEditor, Required]
        public Material material;
    }
}
