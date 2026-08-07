using UnityEngine;

public class int_itemslot : MonoBehaviour
{   
    public bool disable_collider_on_attach = true;


    public Transform t_itemPosition;
    

    // can either be item names OR tags
    public string[] allowed_items;


    // called when an object passes through the slot's trigger
    void OnTriggerEnter(Collider col)
    {
        InteractableObject3D comp = util_interaction.FindInteractionComponent(col.gameObject);
        if (comp == null) {return;}


        if (allowed_items.Length > 0)
        {
            if (comp.GetComponent<int_item>() != null)
            {
                if (!util_items.IsItemAllowed(ItemManager.Instance.items[comp.GetComponent<int_item>().itemType], allowed_items))
                {
                    return;
                }
            } else
            {
                return; // no item component means not allowed
            }
        }


        if (disable_collider_on_attach)
        {
            comp.DisableAllColliders();
        }

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

        GetComponent<Collider>().enabled = false;

        comp.transform.localRotation = Quaternion.identity;
    }
}
