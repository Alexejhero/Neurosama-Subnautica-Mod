using UnityEngine;

#if UNITY_EDITOR
using TriInspector;
#endif

namespace SCHIZO.VFX
{
    public class VFXComponent : MonoBehaviour
    {
        [ReadOnly] public Material material;

        [HideInInspector] public MatPassID matPassID;

#if UNITY_EDITOR
        [OnValueChanged(nameof(SetNewResultTexture))]
        public Texture2D previewImage;

        [PreviewImage] public Texture2D previewResult;

        private void OnValidate()
        {
            if (previewImage)
            {
                if (previewResult == null) SetNewResultTexture();
                if ((previewImage.height != previewResult.height) || (previewImage.width != previewResult.width)) { }
                SetProperties();
                RenderTexture tempResult = RenderTexture.GetTemporary(previewImage.width, previewImage.height, 0, RenderTextureFormat.ARGBHalf);
                Graphics.Blit(previewImage, tempResult, matPassID.ApplyProperties(out int passID), passID);
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
            matPassID ??= new MatPassID(material);
        }

        public virtual void Update()
        {
            SetProperties();
            SchizoVFXStack.Instance.RenderEffect(matPassID);
        }
    }
}
