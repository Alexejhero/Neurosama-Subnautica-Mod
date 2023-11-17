using SCHIZO.VFX;
using UnityEngine;

public class VFX_ARGcensor : MonoBehaviour
{
    public T2DArray t2da;
    public Material material;

    private MatPassID matInstance;
    private Texture2DArray t2darray;

    private readonly int vectorID = Shader.PropertyToID("_ScreenPosition");

    [Range(0f, 1f)]
    public float opacity;

    public float interval = 0.05f;
    private float lastUpdate;
    private float lastRnd = 0f;

    public void Awake()
    {
        t2darray = t2da.PopulateTexture2DArray();
        t2darray.wrapMode = TextureWrapMode.Clamp;
        matInstance = new MatPassID(new Material(material));
        matInstance.mat.SetTexture("_Images", t2darray);
        matInstance.mat.SetFloat("_Strength", opacity);

        _ = SchizoVFXStack.VFXStack;

        lastUpdate = Time.time;
    }

    public void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        if (pos.z > 2)
        {
            if (Time.time - lastUpdate > interval)
            {
                lastRnd = Random.Range(0, t2darray.depth) * Time.timeScale;
                lastUpdate = Time.time;
            }

            matInstance.mat.SetVector( vectorID, new Vector4(pos.x, pos.y, pos.z, lastRnd));
            SchizoVFXStack.RenderEffect(matInstance);
        }
    }
}
