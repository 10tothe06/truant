using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// generic class that goes on the snowmobile, all trailers, etc.

public class SuspendedVehicle : MonoBehaviour
{
    public Rigidbody rb;
    public Transform[] raycastPoints;
    public car_wheel[] wheels; // one for each raycast point

    public float raycastDistance;
    public LayerMask whatIsGround;
    public float vehicleMass;
    public float momentOfInertia;

    [Header("SUSPENSION")]
    public float springCoefficient;
    public float slip_resistance;
    public float spring_absorption;

    [Header("DRAG")]
    public float angularDrag;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].parentVehicle = this;
        }
    }

    void Update()
    {
        // gravity
        rb.linearVelocity += -Vector3.up * Time.deltaTime * 9.81f;

        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < raycastPoints.Length; i++)
        {
            if (Physics.Raycast(raycastPoints[i].position, -transform.up, out hit, raycastDistance, whatIsGround))
            {
                // adding to the linear velocity based on hook's law
                float currentForce = (raycastDistance - hit.distance) * springCoefficient;

                ApplyForce(raycastPoints[i].position, hit.normal * currentForce);

                rb.linearVelocity = MultiplyComponent(rb.linearVelocity, transform.up, 1-spring_absorption);

                wheels[i].SetPosition(hit.point);

                // eliminating sideways friction
                Vector3 c = rb.linearVelocity - Vector3.Project(rb.linearVelocity, hit.normal);
                
                if (c.magnitude > 0.01f)
                {
                    rb.linearVelocity = MultiplyComponent(rb.linearVelocity, c, 1-slip_resistance);
                }
            }
        }


        rb.angularVelocity -= Vector3.Project(rb.angularVelocity, transform.up);

        rb.angularVelocity *= (1-angularDrag);
    }

    public Vector3 MultiplyComponent(Vector3 raw, Vector3 component, float factor)
    {
        Vector3 toReturn = raw - Vector3.Project(raw, component.normalized);

        return toReturn + Vector3.Project(raw, component.normalized) * factor;
    }

    // Vector3 desiredMidpoint = Vector3.zero;
    // Vector3 actualMidpoint = Vector3.zero;

    // RaycastHit hit;
    // for (int i = 0; i < raycastPoints.Length; i++)
    // {
    //     desiredMidpoint += raycastPoints[i].position / raycastPoints.Length;
    //     if (Physics.Raycast(raycastPoints[i].position, -Vector3.up, out hit, raycastDistance, whatIsGround))
    //     {
    //         actualMidpoint += raycastPoints[i].position / raycastPoints.Length;

    //         // adding to the linear velocity based on hook's law
    //         float currentForce = (raycastDistance - hit.distance) * springCoefficient;

    //         currentForce = Mathf.Clamp(currentForce, -maxSpringForce, maxSpringForce);

    //         Vector3 compressFactor = Vector3.Project(rb.linearVelocity, -hit.normal);
    //         rb.linearVelocity -= compressFactor;

    //         rb.linearVelocity += compressFactor * springAbsorptionFactor;

    //         ApplyForce(raycastPoints[i].position, hit.normal * currentForce);
    //     }
    // }

    // rb.linearVelocity *= (1-linearDrag);
    // rb.angularVelocity *= (1-angularDrag);
    
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
