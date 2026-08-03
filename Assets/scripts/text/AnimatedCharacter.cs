using UnityEngine;

public class AnimatedCharacter : MonoBehaviour
{
    public float slide_amplitude;
    public float lerp_speed;
    private Vector3 desired_local_position;

    public void AnimateIn()
    {
        desired_local_position = transform.localPosition;

        transform.localPosition += Vector3.up * slide_amplitude;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desired_local_position, lerp_speed);
    }
}
