using UnityEngine;

// one light component to rule them all


// in all seriousness, this one should be used for all applications

public class int_light : MonoBehaviour
{
    public Light[] light_components;

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
    }
}
