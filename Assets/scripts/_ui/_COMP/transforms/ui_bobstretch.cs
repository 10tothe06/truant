using UnityEngine;

public class ui_bobstretch : MonoBehaviour
{
    public float freq;
    public float amp;

    public float offset;

    public float originalScale;

    public bool animate;

    void Start()
    {
        originalScale = GetComponent<RectTransform>().sizeDelta.x;
    }


    void Update()
    {
        if (animate)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.one * (originalScale + Mathf.Sin((Time.time + offset) * freq) * amp);
        }
        
    }
}
