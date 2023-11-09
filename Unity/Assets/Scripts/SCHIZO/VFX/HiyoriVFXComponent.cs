using SCHIZO.VFX;
using UnityEngine;

public class HiyoriVFXComponent : MonoBehaviour
{
    public Material material;
    private Material matInstance;

    private readonly int vectorID = Shader.PropertyToID("_ScreenPosition");

    public void Awake()
    {
        matInstance = new Material(material);
        SchizoVFXStack stack = SchizoVFXStack.VFXStack;
    }

    public void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        if (pos.z > 0)
        {
            float rnd = Random.Range(-1f, 1f) * Time.timeScale;
            matInstance.SetVector( vectorID, new Vector4(pos.x, pos.y, pos.z, rnd));
            SchizoVFXStack.effectMaterials.Add(matInstance);
        }
    }
}
