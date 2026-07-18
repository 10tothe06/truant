using UnityEngine;

public class ui_bob : MonoBehaviour
{
    public float offset;
    public float freq;
    public float amp;

    public bool animate;

    public Vector3 bobAxis;

    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (animate)
        {
            transform.localPosition = originalLocalPosition + bobAxis * Mathf.Sin(freq * (Time.time + offset)) * amp;
        }
    }
}
