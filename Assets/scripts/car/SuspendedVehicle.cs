using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// generic class that goes on the snowmobile, all trailers, etc.

public class SuspendedVehicle : MonoBehaviour
{
    public Rigidbody rb;
    public Transform[] raycastPoints;

    public float raycastDistance;
    public LayerMask whatIsGround;
    public float vehicleMass;
    public float springCoefficient;
    public float momentOfInertia;

    [Header("DRAG")]
    public float linearDrag;
    public float angularDrag;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 desiredMidpoint = Vector3.zero;
        Vector3 actualMidpoint = Vector3.zero;

        RaycastHit hit;
        for (int i = 0; i < raycastPoints.Length; i++)
        {
            desiredMidpoint += raycastPoints[i].position / raycastPoints.Length;
            if (Physics.Raycast(raycastPoints[i].position, -transform.up, out hit, raycastDistance, whatIsGround))
            {
                actualMidpoint += raycastPoints[i].position / raycastPoints.Length;

                // adding to the linear velocity based on hook's law
                float currentForce = (raycastDistance - hit.distance) * springCoefficient;

                rb.linearVelocity += transform.up * currentForce / vehicleMass;

                ApplyForce(raycastPoints[i].position, transform.up * currentForce);
            }
        }

        rb.linearVelocity *= linearDrag;
        rb.angularVelocity *= angularDrag;
    }
    
    // Apply a force to the ship based on a given offset and direction
    // Forceoffset is a WORLD position
    // force vector is just a direction
    public void ApplyForce(Vector3 forceOffset, Vector3 forceVector)
    {
        //linear velocity
        rb.linearVelocity += forceVector / vehicleMass * Time.deltaTime;

        //angular velocity
        Vector3 a = forceOffset - transform.position;
        Vector3 b = forceVector;

        // angle between the two vectors
        float angle = Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude));

        float r = a.magnitude * Mathf.Sin(angle);

        // the torque force
        float t = b.magnitude * r;
        // the axis of rotation
        Vector3 axis = Vector3.Cross(a.normalized, b.normalized);

        // the acceleration (should be divided by moment of inertia)
        float A = t / momentOfInertia;

        //there is a threshold to make sure that tiny numbers don't sneak in and start rotating the ship (also because a value of NaN will give an error)
        if (r > 0.0005f)
        {
            rb.angularVelocity += axis * A * Time.deltaTime;
        }
    }
}
