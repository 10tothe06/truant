using UnityEngine;

public class ImageEffect : MonoBehaviour
{
    public Material effect;

    // most effects will have some sort of 'amt' value, 
    // so this is a safe bet
    public float strength; 

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        effect.SetFloat("_Amt", strength);
        Graphics.Blit(source, mod, effect);
    }
}
