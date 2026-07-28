using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private SuspendedVehicle sus;
    [Header("DEBUG")]
    public TextMeshProUGUI tx_speed;
    public TextMeshProUGUI tx_steering;
    [Header("CONFIG")]
    public float steeringMoveSpeed;
    public float throttlePercent;
    public float steeringPercent;
    public float steeringForce;
    public float maxSteeringAngle;
    public float wheelForce;

    public bool isBeingDriven;

    [Header("DRAG (overwrites SuspendedVehicle)")]
    public float linearDrag;
    public float angularDrag;
    public float drivingLinearDrag;
    public float drivingAngularDrag;

    void Awake()
    {
        sus = GetComponent<SuspendedVehicle>();
    }

    void Update()
    {
        if (tx_speed != null)
        {
            tx_speed.text = "speed: " + Vector3.Project(sus.rb.linearVelocity, transform.forward).magnitude;

            tx_speed.transform.parent.gameObject.SetActive(isBeingDriven);
        }
        if (tx_steering != null)
        {
            tx_steering.text = "steering: " + steeringPercent;
        }

        if (isBeingDriven)
        {
            throttlePercent = Input.inputAxisForward;

            steeringPercent = Input.inputAxisHorizontal * steeringMoveSpeed;

            sus.angularDrag = drivingAngularDrag;
            //sus.linearDrag = drivingLinearDrag;
        } else
        {
            sus.angularDrag = angularDrag;
            //sus.linearDrag = linearDrag;
        }
        
        //temp 
        Vector3 a = transform.forward * wheelForce * throttlePercent * Time.deltaTime;
        GetComponent<Rigidbody>().linearVelocity += a;
        GetComponent<Rigidbody>().angularVelocity += Vector3.up * Vector3.Dot(a, transform.forward) * steeringForce * steeringPercent;
    }

    public void EnterDriver()
    {
        isBeingDriven = true;
    }

    public void ExitDriver()
    {
        isBeingDriven = false;
    }
}
