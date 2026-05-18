using UnityEngine;
// reworked 12/13/2025

/*
INFO:

if you have an interactable object with multiple colliders,
or one where the InteractableObject3D class cannot be on the same object as the collider,
then this class can be placed ON THE COLLIDER OBJECT and it'll point back to the InteractableObject3D class
*/


public class InteractCollider : MonoBehaviour
{
    public InteractableObject3D parentObject;
}
