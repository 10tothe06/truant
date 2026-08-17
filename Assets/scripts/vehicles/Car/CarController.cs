using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    private SuspendedVehicle sus;

    public float drive_force;
    public float turn_coefficient;

    public int_chair driver_seat;
    public InteractableObject3D ignition_key;

    
    public bool is_engine_started;

    public int_fluidtank fuel_tank;



    [Header("EVENTS")]
    // used as a sort of trigger for certain levels
    public UnityEvent onEngineStart; 



    void Awake()
    {
        sus = GetComponent<SuspendedVehicle>();
        rb = GetComponent<Rigidbody>();

        // adding the proper events to objects
        ignition_key.onInteract.AddListener(OnIgnitionKeyPressed);
        
        // basically just a state reset
        KillEngine();
    }

    void Start()
    {
        ui_debugmenu.AddEntry("car_fuel", () => fuel_tank.tank_fill_level.ToString());
    }

    void Update()
    {
        if (driver_seat.isSitting && is_engine_started)
        {
            if (Input.inputAxisForward > 0)
            {
                rb.linearVelocity += transform.right * Time.deltaTime * drive_force;
            }

            rb.angularVelocity += Vector3.up * Input.inputAxisHorizontal * turn_coefficient * Time.deltaTime * util_math.ProjectedMagnitude(rb.linearVelocity, transform.right);
        }
    }

    private void OnIgnitionKeyPressed()
    {
        if (is_engine_started)
        {
            KillEngine();
        } else
        {
            StartEngine();
        }
    }

    public void KillEngine()
    {
        Debug.Log("Car engine stopped.");

        is_engine_started = false;


        ignition_key.hoverPrompt = "Press E to start engine.";
    }

    public void StartEngine()
    {
        Debug.Log("Car engine started.");


        is_engine_started = true;

        // TODO: play the sound sequence


        ignition_key.hoverPrompt = "Press E to kill engine.";

        onEngineStart.Invoke();
    }
}
