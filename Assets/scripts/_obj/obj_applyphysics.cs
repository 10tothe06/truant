using UnityEngine;

// this script should realistically be applied to every object

// doing so allows me to do a few things,
// like pausing the game in a more flexible way than using the built-in physics update system
// buoyancy, too, is controlled from here
// freezing/unfreezing objects is here too

public class obj_applyphysics : MonoBehaviour
{
    [Header("General Settings")]
    public float mass = 1f;

    [Header("Buoyancy Settings")]
    


    public bool useGravity = true;
    public obj_generic gComp;
    private Rigidbody rb;
    public Vector3 gravityDirection = -Vector3.right;
    public float gravitationalAcceleration = 0.981f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gComp = GetComponent<obj_generic>();

        gComp.onEntityUpdate.AddListener(EntityUpdate);


        // just making sure:
        rb.useGravity = false;
        rb.angularDamping = 0;
    }

    void EntityUpdate()
    {
        if (rb.isKinematic) {return;}
        
        if (useGravity)
        {
            rb.linearVelocity += gravityDirection * gravitationalAcceleration * Time.deltaTime;
        }
    }

    public void Freeze()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreeze()
    {
        rb.constraints = RigidbodyConstraints.None;
    }
}
