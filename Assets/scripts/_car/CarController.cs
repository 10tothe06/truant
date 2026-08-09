using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    private SuspendedVehicle sus;

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
