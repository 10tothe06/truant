using UnityEngine;
using UnityEngine.Events;

public class poi_generic : MonoBehaviour
{
    [Header("PLACEMENT SETTINGS")]
    public bool must_be_lakeside;

    public float exclusion_radius;
    public Transform exclusion_debug_object;

    
    [Header("UNITY EVENTS")]
    public UnityEvent onInitialize;

    void OnDrawGizmos()
    {
        if (exclusion_debug_object != null)
        {
            // visualizing the exclusion radius with a cirlce

            exclusion_debug_object.localScale = Vector3.one * exclusion_radius * 2;
        }
    }

    
    // called by the world manager when the object is spawned in
    public void Initialize()
    {
        onInitialize.Invoke();

        if (must_be_lakeside)
        {
            // every poi starts off with a random placement,
            // but some POIs NEED to be on the shoreline

            // so we gotta take the position that we're given and move it until its by the lake
            // we're doing this in a bit of a silly way
            
            transform.position = util_map.GetLakesidePosition(transform.position);
        }
    }
}
