using System;
using UnityEngine;

public class int_jiggle : MonoBehaviour
{
    private Vector3 defaultPosition;
    private float jiggle_amount;
    public float jiggle_fade_speed = 0.2f;

    void Awake()
    {
        defaultPosition = transform.localPosition;
    }

    public void Jiggle(float amt = 0.1f)
    {
        jiggle_amount = amt;
    }
    
    void Update()
    {
        if (jiggle_amount > 0)
        {
            jiggle_amount = Mathf.Lerp(jiggle_amount, 0, jiggle_fade_speed);

            Vector3 jiggle_offset = new Vector3(
                UnityEngine.Random.Range(-jiggle_amount, jiggle_amount), 
                UnityEngine.Random.Range(-jiggle_amount, jiggle_amount), 
                UnityEngine.Random.Range(-jiggle_amount, jiggle_amount));

            transform.localPosition = defaultPosition + jiggle_offset;
        }
    }
}
