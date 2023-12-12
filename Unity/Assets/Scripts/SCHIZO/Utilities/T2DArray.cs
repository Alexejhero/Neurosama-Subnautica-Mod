using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEditor;

namespace SCHIZO.Utilities
{
    [CreateAssetMenu(menuName ="SCHIZO/Utilities/Texture2D Array")]
    public class T2DArray : ScriptableObject
    {
        [HideInInspector]
        public Texture2DArray array;

#if UNITY_EDITOR

        [Required, ValidateInput(nameof(ValidateTextures))]
        public List<Texture> textures;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public FilterMode filterMode = FilterMode.Point;
        [Space]
        [ShowIf(nameof(array), null)]
        public TextureCompressionQuality compressionQuality;

        [ShowInInspector, ShowIf(nameof(array), null), ReadOnly]
        public TextureFormat compressionFormat;

        private void OnValidate()
        {
            if (array)
            {
                array.wrapMode = wrapMode;
                array.filterMode = filterMode;
            }
        }

        private bool validtationPass = false;
        private TriValidationResult ValidateTextures()
        {
            if (textures.Count > 1) textures.RemoveAll(o => o == null);

            if (textures.Count > 1 && textures[0] != null)
            {
                int w = textures[0].width;
                int h = textures[0].height;
                TextureFormat format = ((Texture2D) textures[0]).format;

                foreach (Texture texture in textures)
                {
                    if (texture.width != w || texture.height != h )
                    {
                        validtationPass = false;
                        return TriValidationResult.Error("All Textures must have same dimensions!");
                    }
                    if(((Texture2D) texture).format != format)
                    {
                        validtationPass = false;
                        return TriValidationResult.Error("All Textures must have same format!");
                    }
                }
                validtationPass = true;
                compressionFormat = format;
                return TriValidationResult.Valid;
            }
            validtationPass = true;
            return TriValidationResult.Warning("Please assign at least 2 textures, or use Texture2D if single texture is intended");
        }

        // for some reason Texture2DArray sub asset does not update when overwriting it, so uuh it's a one time thing i guess
        [Button, ShowIf(nameof(array), null), ShowIf(nameof(validtationPass),true)]
        private void GenerateArray()
        {
            if (textures.Count == 0 || textures[0] == null) return;

            if (array == null)
            {
                array = PopulateTexture2DArray();
                AssetDatabase.AddObjectToAsset(array, this);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        private Texture2DArray PopulateTexture2DArray()
        {
            List<Texture> tt = textures;

            Texture2DArray t2da = new Texture2DArray(tt[0].width, tt[0].height, tt.Count, compressionFormat, false, true);
            
            for (int i = 0; i < tt.Count; i++)
            {
                Graphics.CopyTexture(tt[i], 0,0, t2da, i,0);
            }
            t2da.wrapMode = wrapMode;
            t2da.filterMode = filterMode;

            return t2da;
        }
#endif
    }
}
