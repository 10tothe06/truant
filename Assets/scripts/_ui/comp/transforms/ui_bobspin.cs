using UnityEngine;

public class ui_bobspin : MonoBehaviour
{
    public float freq;
    public float amp;

    public float offset;

    public float originalAngle;

    public bool animate;

    void Start()
    {
        originalAngle = GetComponent<RectTransform>().eulerAngles.z;
    }


    void Update()
    {
        if (animate)
        {
            GetComponent<RectTransform>().eulerAngles = Vector3.forward * Mathf.Sin((Time.time + offset) * freq) * amp;
        }
        
    }
}
