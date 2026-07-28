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
    public bool isDraggable = true;
    
    [Header("Config")]
    //public bool logInteractionEvents;
    public string hoverPrompt; // might change this for a more robust system, but it certainly works for now

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent<GameObject> onInteractByObject;

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
}
