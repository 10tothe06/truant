using UnityEngine;

public class int_bullet : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }



    public void OnShoot(float muzzle_velocity)
    {
        rb.linearVelocity = transform.forward * muzzle_velocity;
    }
}
