using UnityEngine;

public class int_itemslot : MonoBehaviour
{   
    public Transform t_itemPosition;


    // called when an object passes through the slot's trigger
    void OnTriggerEnter(Collider col)
    {
        InteractableObject3D comp = util_interaction.FindInteractionComponent(col.gameObject);
        if (comp == null) {return;}

        // dealing with things if the player is dragging or carrying the object
        if (Player.isDraggingObject)
        {
            if (Player.interaction_source.objectToDrag.GetComponent<InteractableObject3D>() == comp)
            {
                Player.interaction_source.StopDraggingObject();
            }
        }
        if (Player.isCarryingObject)
        {
            
        }

        comp.transform.SetParent(t_itemPosition);
        comp.DisablePhysics();
        comp.transform.localPosition = Vector3.zero;

        comp.transform.localRotation = Quaternion.identity;
    }
}
