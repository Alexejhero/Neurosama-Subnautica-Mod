using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using TriInspector;
#endif

namespace SCHIZO.Utilities
{
    [CreateAssetMenu(menuName = "SCHIZO/Utilities/Texture2D Array")]
    public class T2DArray : ScriptableObject
    {
        [HideInInspector] public Texture2DArray array;

#if UNITY_EDITOR

        [Required, ValidateInput(nameof(ValidateTextures))]
        public List<Texture> textures;

        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public FilterMode filterMode = FilterMode.Point;
        [Space]
        public TextureCompressionQuality compressionQuality;
        [ReadOnly]
        public TextureFormat compressionFormat;

        private bool validationPass = false;
        private void OnValidate()
        {
            if (array)
            {
                array.wrapMode = wrapMode;
                array.filterMode = filterMode;
            }
        }

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
                    if (texture.width != w || texture.height != h)
                    {
                        validationPass = false;
                        return TriValidationResult.Error("All Textures must have same dimensions!");
                    }

                    if (((Texture2D) texture).format != format)
                    {
                        validationPass = false;
                        return TriValidationResult.Error("All Textures must have same format!");
                    }
                }

                validationPass = true;
                compressionFormat = format;
                return TriValidationResult.Valid;
            }

            validationPass = true;
            return TriValidationResult.Warning("Please assign at least 2 textures, or use Texture2D if single texture is intended");
        }


        [Button, ShowIf(nameof(validationPass), true)]
        private void GenerateArray()
        {
            if(textures.Count == 0 || textures[0] == null) return;
            PopulateTexture2DArray();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private bool CanUpdateAlreadyCreatedArray => array && array.depth == textures.Count && array.height == textures[0].height && array.width == textures[0].width;
        
        private void PopulateTexture2DArray()
        {
            if (!array)
            {
                array = new(textures[0].width, textures[0].height, textures.Count, compressionFormat, false, true);
                AssetDatabase.AddObjectToAsset(array, this);
            }

            if (!CanUpdateAlreadyCreatedArray)
            {
                AssetDatabase.RemoveObjectFromAsset(array);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                array = new(textures[0].width, textures[0].height, textures.Count, compressionFormat, false, true);
                AssetDatabase.AddObjectToAsset(array, this);
            }

            for (int i = 0; i < textures.Count; i++)
            {
                Graphics.CopyTexture(textures[i], 0, 0, array, i, 0);
            }

            array.wrapMode = wrapMode;
            array.filterMode = filterMode;
        }
#endif
    }
}
