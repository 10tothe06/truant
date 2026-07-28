using UnityEngine;
using UnityEngine.InputSystem;

// positioned on the player's side
//  the PlayerController.cs script is busy enough as it is

public class int_objectcarrier : MonoBehaviour
{
    public Transform carryParent;

    public bool isCarryingObject;
    
    void Update()
    {
        if (isCarryingObject)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                DropObject();
            }
        }
    }

    // picking up an object
    public void CarryObject(GameObject g)
    {
        // set the parent and remove physics
        g.transform.SetParent(carryParent);
        if (g.GetComponent<Rigidbody>() != null)
        {
            g.GetComponent<Rigidbody>().isKinematic = true;
        }

        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;

        isCarryingObject = true;

        // disable the object's colliders
        if (g.GetComponent<int_colliderlist>() != null)
        {
            g.GetComponent<int_colliderlist>().DisableAll();
        }
    }

    public void DropObject()
    {
        GameObject g = carryParent.GetChild(0).gameObject;

        isCarryingObject = false;
        g.transform.SetParent(null); // TODO: not null, but back in the entity container

        if (g.GetComponent<Rigidbody>() != null)
        {
            g.GetComponent<Rigidbody>().isKinematic = false;
        }

        // re-enable the object's colliders
        if (g.GetComponent<int_colliderlist>() != null)
        {
            g.GetComponent<int_colliderlist>().EnableAll();
        }

        g.GetComponent<int_carryable>().OnDrop();
        g.GetComponent<int_carryable>().DropCooldown();
    }
}
