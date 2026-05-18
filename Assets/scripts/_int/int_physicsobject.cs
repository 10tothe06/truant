using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
// reworked 12/13/2025

// thanks to my work on Drivetools, I've learned how useful it is to have classes that build off of a generic class
// this class will automatically add itself to the interaction events of the InteractableObject3D class

/*
INFO:

place this on any physics object

there are no update calls because that's controlled by the InteractableObject3D

*/

[RequireComponent(typeof(InteractableObject3D))]
public class int_physicsobject : MonoBehaviour
{
    private InteractableObject3D interactComponent;
    public bool enableImpactSounds = true;

    [Header("Info")]
    // is the object currently held by the player?
    public bool isHeld;
    public bool grabbedThisFrame; // helping with the timing of the unity events

    [Header("Events")]
    // these two are only called if the object can be "held" by the player
    public UnityEvent onPickup; // called upon picking up the object
    public UnityEvent onDrop; // called when the player drops this object

    // maybe add the tag system back?

    void Awake()
    {
        interactComponent = GetComponent<InteractableObject3D>();
        if (interactComponent != null)
        {
            interactComponent.onInteract.AddListener(Interact);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (enableImpactSounds)
        {
            Vector3 v = GetComponent<Rigidbody>().linearVelocity;
            Material impactMaterial = util_audio.GetMaterialFromRay(transform.position, v, 10f, 0);
            //AudioManager.Instance.PlaySound(utils_audio.GetClipFromMaterial(impactMaterial, InteractionHandler3D.Instance.impactSounds, InteractionHandler3D.Instance.defaultImpactSound));
        }
    }

    void Update() {
        grabbedThisFrame = false;
    }

    public void Interact()
    {
        if (!isHeld)
        {
            TryGrab();
        } else {Drop();}
    }

    public void Drop()
    {
        isHeld = false;
        onDrop.Invoke(); // extending to even more calls
        gameObject.tag = "Interact";  // useful in case I need to check this later

        InteractionHandler3D.Instance.t_heldObject = null;
    }

    public void TryGrab()
    {
        isHeld = true;
        grabbedThisFrame = true;
        gameObject.tag = "HeldByPlayer"; // useful in case I need to check this later
        onPickup.Invoke(); // extending to even more calls

        if (InteractionHandler3D.Instance.t_heldObject == null) // can't grab if already holding something
        {
            InteractionHandler3D.Instance.t_heldObject = transform;
        }
    }
}
