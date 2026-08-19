using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

// one light component to rule them all


// in all seriousness, this one should be used for all applications
// (basically just a wrapper for the built-in unity Light component)

public class int_light : MonoBehaviour
{
    public Light[] light_components;

    public float[] default_intensities;

    public bool start_as_on;


    // bool can only be set by calling the switch on/off functions
    public bool is_on {get; private set;}


    [Header("UNITY EVENTS")]
    public UnityEvent onSwitchOn; // lwk terrible name for this
    public UnityEvent onSwitchOff;



    void Awake()
    {
        if (light_components.Length == 0)
        {
            
            // filling out the light array just with the one on the same object
            if (GetComponent<Light>() != null)
            {
                light_components = new Light[] {GetComponent<Light>()};
            }
        }

        if (default_intensities.Length != light_components.Length)
        {
            default_intensities = new float[light_components.Length];


            for (int i = 0; i < light_components.Length; i++)
            {
                if (light_components[i].intensity != 0)
                {
                    default_intensities[i] = light_components[i].intensity;
                } else
                {
                    default_intensities[i] = 1f;
                }
            }
        }

        if (start_as_on)
        {
            SwitchOn();
        } else
        {
            SwitchOff();
        }
    }

    public void SwitchOn() {
        is_on = true;


        for (int i = 0; i < light_components.Length; i++)
        {
            light_components[i].intensity = default_intensities[i];
        }
    }

    public void SwitchOff() {
        is_on = false;

        
        for (int i = 0; i < light_components.Length; i++)
        {
            light_components[i].intensity = 0f;
        }
    }
}
