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

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent<GameObject> onInteractByObject;

    void Awake()
    {
        has_physics = GetComponent<Rigidbody>() != null;
        
        if (auto_colliders)
        {
            collider_list = GetComponentsInChildren<Collider>();
        }
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
        if (GetComponent<obj_applyphysics>() != null)
        {
            GetComponent<obj_applyphysics>().useGravity = false;
        }
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    public void EnablePhysics()
    {
        has_physics = true;
        if (GetComponent<obj_applyphysics>() != null)
        {
            GetComponent<obj_applyphysics>().useGravity = true;
        }
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
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
