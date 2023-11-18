using SCHIZO.VFX;
using UnityEngine;

public class VFX_ARGCensor : MonoBehaviour
{
    public T2DArray t2da;
    public Material material;

    private MatPassID matInstance;
    private Texture2DArray t2darray;

    private readonly int vectorID = Shader.PropertyToID("_ScreenPosition");
    private readonly int scaleID = Shader.PropertyToID("_Scale");

    [Range(0f, 1f)]
    public float opacity;

    public float scale = 2f;

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
        Vector3 dirToCam = (Camera.main.transform.position - transform.position).normalized;
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + dirToCam);

        float dot = Vector3.Dot(transform.forward, dirToCam);

        if (pos.z > 0 && dot > -0.3f)
        {
            if (Time.time - lastUpdate > interval)
            {
                lastRnd = Random.Range(0, t2darray.depth - 1) * Time.timeScale;
                lastUpdate = Time.time;
            }

            matInstance.mat.SetVector(vectorID, new Vector4(pos.x, pos.y, pos.z, lastRnd));
            matInstance.mat.SetFloat(scaleID, scale);
            SchizoVFXStack.RenderEffect(matInstance);
        }
    }
}
