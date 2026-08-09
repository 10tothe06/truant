using UnityEngine;

public class int_itemslot : MonoBehaviour
{   
    public bool is_holding_item {get; private set;}
    public bool disable_collider_on_attach = true;


    public Transform t_itemPosition;
    

    // can either be item names OR tags
    public string[] allowed_items;

    private InteractableObject3D currently_held_object;

    public void TryPlaceItemFromPlayer()
    {
        TryPlaceItem(Player.DropSelectedItem().GetComponent<InteractableObject3D>());
    }


    // called when an object passes through the slot's trigger
    void OnTriggerEnter(Collider col)
    {
        InteractableObject3D comp = util_interaction.FindInteractionComponent(col.gameObject);
        if (comp == null) {return;}


        TryPlaceItem(comp);
    }

    private void TryPlaceItem(InteractableObject3D comp)
    {
        if (allowed_items.Length > 0)
        {
            if (comp.GetComponent<int_item>() != null)
            {
                if (!util_items.IsItemAllowed(ItemManager.Instance.items[comp.GetComponent<int_item>().item_data.itemIndex], allowed_items))
                {
                    return;
                }
            } else
            {
                return; // no item component means not allowed
            }
        }


        PlaceItem(comp);
    }   


    // slots work the same as the player,
    // where we have to get the item to drop before doing shit with it

    // basically the reverse of the placing function
    public InteractableObject3D DropItem()
    {
        Debug.Log("b");


        // there are some edge cases where we don't want colliders, I guess?
        // not gonna worry about it
        currently_held_object.EnableAllColliders();
        currently_held_object.SetCollidersToSolid();

        currently_held_object.transform.SetParent(null);
        currently_held_object.EnablePhysics();

        currently_held_object.parent_slot = null;

        InteractableObject3D io = currently_held_object;
        currently_held_object = null;

        GetComponent<Collider>().enabled = true;

        is_holding_item = false;

        return io;
    }

    private void PlaceItem(InteractableObject3D comp)
    {
        Debug.Log('a');
        comp.parent_slot = this;
        currently_held_object = comp;

        if (disable_collider_on_attach)
        {
            comp.SetCollidersToTrigger();
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

        is_holding_item = true;
    }
}
