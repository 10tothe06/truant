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
    [Header("Config")]
    public bool logInteractionEvents;
    public string hoverPrompt; // might change this for a more robust system, but it certainly works for now

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent onPeriodic;

    public void HandleInteract()
    {
        onInteract.Invoke();

        if (logInteractionEvents) Debug.Log("interacted with " + gameObject.name);
    }

    public void HandlePeriodic()
    {
        onPeriodic.Invoke();
    }

    // we COULD call this from the InteractionHandler class, 
    // but for now I think its unecessary to try and keep track of all interactable objects in the scene
    void Update(){HandlePeriodic(); }
}
