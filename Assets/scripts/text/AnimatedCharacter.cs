using NUnit.Framework;
using TMPro;
using UnityEngine;

public class AnimatedCharacter : MonoBehaviour
{
    public float slide_amplitude = 20f;
    public float slide_lerp_speed = 0.5f;

    public float fade_lerp_speed = 0.1f;

    private Vector3 desired_local_position;
    private float desired_opacity;
    private bool isSliding;
    private bool isFading;
    private TextMeshProUGUI[] text_components;


    #region FADING

    public void FadeOut(bool leave_hightlights)
    {
        if (!GetComponent<LayeredText>().layering_enabled || !leave_hightlights)
        {
            isFading = true;
            desired_opacity = 0;
        }   
    }

    public void FadeIn()
    {
        
    }

    #endregion



    public void AnimateIn(bool enable_slide = false, bool enable_fade = false)
    {
        isSliding = enable_slide;
        isFading = enable_fade;

        desired_local_position = transform.localPosition;
        desired_opacity = 1;

        text_components = GetComponentsInChildren<TextMeshProUGUI>();

        if (enable_slide)
        {
            transform.localPosition += Vector3.up * slide_amplitude;
        }

        if (enable_fade)
        {
            for (int i = 0; i < text_components.Length; i++)
            {
                text_components[i].color = new Color(text_components[i].color.r, text_components[i].color.g, text_components[i].color.b, 0);
            }
        }
    }

    void Update()
    {
        if (isSliding)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, desired_local_position, slide_lerp_speed);
        }
        if (isFading)
        {
            for (int i = 0; i < text_components.Length; i++)
            {
                text_components[i].color = new Color(text_components[i].color.r, text_components[i].color.g, text_components[i].color.b, Mathf.Lerp(text_components[i].color.a, desired_opacity, fade_lerp_speed));
            }
        }
    }
}
