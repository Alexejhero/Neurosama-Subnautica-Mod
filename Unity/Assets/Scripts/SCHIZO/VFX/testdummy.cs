using UnityEngine;

public class testdummy : MonoBehaviour
{
    public Material material;
    public Transform target;

    private void Update()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);
        material.SetVector("_ScreenPosition", new Vector4(pos.x, pos.y, 1, 0));

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
