using UnityEngine;

#if UNITY_EDITOR
using TriInspector;
#endif

namespace SCHIZO.VFX
{
    public abstract class VFXComponent : MonoBehaviour
    {
        // ReSharper disable once RedundantNameQualifier
        [global::TriInspector.ReadOnly] public Material material;

        [HideInInspector] public CustomMaterialPropertyBlock propBlock;

#if UNITY_EDITOR
        [OnValueChanged(nameof(SetNewResultTexture))]
        public Texture2D previewImage;

        [PreviewImage] public Texture2D previewResult;

        private void OnValidate()
        {
            if (previewImage)
            {
                if (previewResult == null || ((previewImage.height != previewResult.height) || (previewImage.width != previewResult.width))) SetNewResultTexture();
                SetProperties();
                RenderTexture tempResult = RenderTexture.GetTemporary(previewImage.width, previewImage.height, 0, RenderTextureFormat.ARGBHalf);
                Graphics.Blit(previewImage, tempResult, propBlock.ApplyProperties(out int passID), passID);
                Graphics.ConvertTexture(tempResult, previewResult);
                tempResult.Release();
            }
        }

        private void SetNewResultTexture()
        {
            previewResult = new Texture2D(previewImage.width, previewImage.height, TextureFormat.RGBAHalf, false);
        }
#endif

        public virtual void SetProperties()
        {
            propBlock ??= new CustomMaterialPropertyBlock(material);
        }

        public virtual void Update()
        {
            SetProperties();
            SchizoVFXStack.Instance.RenderEffect(propBlock);
        }
    }
}
