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
        [InfoBox("All Textures must have same dimensions!", TriMessageType.Error, nameof(texturesDimensionsMismatch))]
        [InfoBox("Please assign at least 2 textures, or use Texture2D if single texture is intended", TriMessageType.Warning, nameof(tooFewTextures))]
        [Required]
        public List<Texture> textures;

        public TextureCompressionQuality compressionQuality;
        public TextureWrapMode wrapMode;
        public FilterMode filterMode;

        private bool texturesDimensionsMismatch = true;
        private bool tooFewTextures = true;

        private void OnValidate()
        {
            if (textures.Count > 1 ) textures.RemoveAll(o => o == null);
            tooFewTextures = textures.Count < 2;

            if (textures.Count > 1 && textures[0] != null)
            {
                texturesDimensionsMismatch = false;
                int w = textures[0].width;
                int h = textures[0].height;

                foreach (Texture texture in textures)
                {
                    if (texture.width != w || texture.height != h)
                    {
                        texturesDimensionsMismatch = true;
                        return;
                    }
                }
            }
        }

        [Button]
        private void GenerateArray()
        {
            if (textures.Count == 0 ) return;

            if (!array)
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
            Texture2DArray t2da = new Texture2DArray(tt[0].width, tt[0].height, tt.Count, TextureFormat.DXT5, false);

            for (int i = 0; i < tt.Count; i++)
            {
                EditorUtility.CompressTexture((Texture2D) tt[i], TextureFormat.DXT5, compressionQuality);
                Graphics.CopyTexture(tt[i], 0, t2da, i);
            }
            t2da.wrapMode = wrapMode;
            t2da.filterMode = filterMode;

            return t2da;
        }
#endif
    }
}
