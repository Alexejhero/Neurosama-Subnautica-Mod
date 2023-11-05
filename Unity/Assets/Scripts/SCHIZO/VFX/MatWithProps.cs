using UnityEngine;

namespace SCHIZO.VFX
{
    public sealed class MatWithProps
    {
        public MatWithProps(Material material)
        {
            this.material = material;
        }
        public MatWithProps(Material material, int vectorPropertyID, Vector4 vectorPropertyValue)
        {
            this.material = material;
            this.vectorPropertyID = vectorPropertyID;
            this.vectorPropertyValue = vectorPropertyValue;
        }

        public MatWithProps(Material material, int colorPropertyID, Color colorPropertyValue)
        {
            this.material = material;
            this.colorPropertyID = colorPropertyID;
            this.colorPropertyValue = colorPropertyValue;
        }
        public MatWithProps(Material material, int vectorPropertyID, Vector4 vectorPropertyValue, int colorPropertyID, Color colorPropertyValue)
        {
            this.material = material;
            this.vectorPropertyID = vectorPropertyID;
            this.vectorPropertyValue = vectorPropertyValue;
            this.colorPropertyID = colorPropertyID;
            this.colorPropertyValue = colorPropertyValue;
        }

        public Material material;
        public int vectorPropertyID = -1;
        public Vector4 vectorPropertyValue = Vector4.zero;
        public int colorPropertyID = -1;
        public Color colorPropertyValue = Color.white;
    }
}
