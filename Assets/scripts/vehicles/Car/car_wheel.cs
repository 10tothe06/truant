using UnityEngine;

public class car_wheel : MonoBehaviour
{
    public SuspendedVehicle parentVehicle;
    public bool shouldSpin;

    public Transform t_wheelEdge;

    private Vector3 default_local_position;
    [HideInInspector]
    public float wheel_radius;

    private Vector3 old_world_position;

    void Awake()
    {
        default_local_position = transform.localPosition;

        wheel_radius = t_wheelEdge.localPosition.magnitude;
    }

    public void SetPosition(Vector3 ground_point)
    {
        transform.position = ground_point;

        transform.position += (default_local_position - transform.localPosition).normalized * wheel_radius;
    }

    void Update()
    {
        if (shouldSpin && parentVehicle != null)
        {
            float distance_travelled = -util_math.ProjectedMagnitude(transform.position - old_world_position, parentVehicle.transform.right);

            transform.Rotate(Vector3.forward * distance_travelled / wheel_radius / Time.deltaTime, Space.Self);

            old_world_position = transform.position;
        }
    }
}
