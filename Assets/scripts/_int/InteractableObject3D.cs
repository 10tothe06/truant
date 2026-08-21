using UnityEngine;
using UnityEngine.Events;
// reworked 12/13/2025

// script for ALL interactable objects, 
// allows communication between the interaction handler and specific int_ classes

/*
INFO:

any interactable objecets must have this component

ideally this script is placed on the object with the collider,
but the InteractCollider class exists so that that's not necessary
(see InteractCollider.cs)
*/

public class InteractableObject3D : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Physics Settings")]
    public float momentOfInertia;
    public float buoyancy_coefficient;
    public float buoyancy_force_limit;

    public float linearDamping = 0.99f;
    public float angularDamping = 0.99f;


    [Space(30)]
    [HideInInspector]
    public int_itemslot parent_slot;

    public bool can_be_interacted_with = true;


    public bool auto_colliders = true;
    public Collider[] collider_list;

    public bool has_physics = true;


    [Space(14)]
    public bool isDraggable = true;
    
    [Header("Config")]
    //public bool logInteractionEvents;
    public string hoverPrompt; // might change this for a more robust system, but it certainly works for now




    [Header("INFORMATION")]
    public bool is_in_water {get; private set;}


    private Vector3 stored_linear_velocity;
    private Vector3 stored_angular_velocity;


    [Space(20)]
    [Header("SOUNDS")]
    public string impact_sound;


    [Header("Events")]
    // when the object is interacted with
    public UnityEvent onInteract;
    // same, but provides the source of the interaction
    public UnityEvent<GameObject> onInteractByObject;


    // when the object (must be an item) is inspected
    public UnityEvent onInspectObject;
    // when the object is DONE inspecting
    public UnityEvent onFinishInspecting;

    // exactly what it sounds like
    public UnityEvent onEnterWater;
    // when the object is hit by something,
    // OR HITS SOMETHING (both ways)
    // <float> var is the impact acceleration (delta-v)
    public float impact_threshold = 0f; // what actually counts as an impact in the first place
    public UnityEvent<float> onImpact;
        


    void Awake()
    {
        has_physics = GetComponent<Rigidbody>() != null;
        
        if (auto_colliders)
        {
            collider_list = GetComponentsInChildren<Collider>();
        }

        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GameManager.Instance.onGamePaused.AddListener(() => DisablePhysics());
        GameManager.Instance.onGameResume.AddListener(() => EnablePhysics());
    }

    void Update()
    {
        HandleBuoyancy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > impact_threshold)
        {
            onImpact.Invoke(collision.impulse.magnitude);

            if (!string.IsNullOrEmpty(impact_sound))
            {
                AudioManager.PlaySound(impact_sound, transform.position);
            }
        }
    }

    // also updates the is_in_water_variable
    void HandleBuoyancy()
    {
        is_in_water = false;

        if (rb == null) {return;}
        if (rb.isKinematic) {return;}

        float forceAmt = 0;

        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 50f, -Vector3.up, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("WaterSurface")))
        {
            
        } else
        {
            // no water, so no buoyancy
            return;
        }

        if (hit.point.y > transform.position.y)
        {
            is_in_water = true;

            // calculating the buoyancy force
            forceAmt = hit.point.y - transform.position.y;
            forceAmt = Mathf.Clamp(forceAmt * forceAmt, 0, buoyancy_force_limit) * buoyancy_coefficient;

            rb.linearVelocity *= linearDamping;
            rb.angularVelocity *= angularDamping;
        }

        rb.linearVelocity += Vector3.up * forceAmt / rb.mass;
    }

    public void SetCollidersToSolid()
    {
        for (int i = 0; i < collider_list.Length; i++)
        {
            collider_list[i].isTrigger = false;
        }
    }
    public void SetCollidersToTrigger()
    {
        for (int i = 0; i < collider_list.Length; i++)
        {
            collider_list[i].isTrigger = true;
        }
    }

    public void DisablePhysics()
    {
        has_physics = false;
        
        if (rb != null)
        {
            stored_angular_velocity = rb.angularVelocity;
            stored_linear_velocity = rb.linearVelocity;

            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    public void EnablePhysics(bool apply_stored_velocities = true)
    {
        
        has_physics = true;
        
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;

            if (apply_stored_velocities)
            {
                rb.linearVelocity = stored_linear_velocity;
                rb.angularVelocity = stored_angular_velocity;
            }
        }
    }

    public void DisableAllColliders()
    {
        SetAllColliders(false);
    }
    public void EnableAllColliders()
    {
        SetAllColliders(true);
    }
    private void SetAllColliders(bool enable) {
        for (int i = 0; i < collider_list.Length; i++)
        {
            collider_list[i].enabled = enable;
        }
    }

    public void HandleInteract()
    {
        onInteract.Invoke();

        //if (logInteractionEvents) Debug.Log("interacted with " + gameObject.name);
    }

    public void HandleInteractByObject(GameObject g)
    {
        onInteract.Invoke();
        onInteractByObject.Invoke(g);

        //if (logInteractionEvents) Debug.Log("interacted with " + gameObject.name);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
