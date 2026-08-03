using UnityEngine;


// a simple script for applying shaders to a camera

public class ImageEffect : MonoBehaviour
{
    public Material effect;

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        Graphics.Blit(source, mod, effect);
    }
}
