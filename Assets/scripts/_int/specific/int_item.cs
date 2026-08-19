using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class int_item : MonoBehaviour
{
    [Header("this is an overwrite, not needed")]
    [Header("if null it uses the gameobject name")]
    public string item_type;

    public inv_itemstack item_data;

    public UnityEvent onDataUpdate;
    public UnityEvent onInitialize;

    private bool is_initialized;


    void Start()
    {
        GetComponent<InteractableObject3D>().onInteract.AddListener(Pickup);

        if (!is_initialized)
        {
            item_data = new inv_itemstack();
            item_data.itemCount = 1;
            if (string.IsNullOrEmpty(item_type))
            {
                item_type = gameObject.name;
                item_data.SetItemIndex(ItemManager.GetItemIndexFromName(gameObject.name));
            } else
            {
                item_data.SetItemIndex(ItemManager.GetItemIndexFromName(item_type));
            }

            onInitialize.Invoke();
            is_initialized = true;
        }
    }

    public void SetItemData(inv_itemstack data)
    {
        item_data = data;
        item_type = gameObject.name;
        item_data.cellIndex = 0;
        item_data.itemCount = 1;

        onDataUpdate.Invoke();
        onInitialize.Invoke();
        is_initialized = true;
    }

    public void Pickup()
    {
        bool canFitItem = false;


        // figuring out whether the player has the inventory space for this item
        for (int i = 0; i < Player.player_inventory.cellsTaken.Length; i++)
        {
            item_data.cellIndex = i;
            if (Player.player_inventory.CanFitItem(item_data))
            {
                canFitItem = true;
                break;
            }
        }

        if (canFitItem)
        {
            Player.GiveItem(item_data);

            if (GetComponent<InteractableObject3D>().parent_slot != null)
            {
                GetComponent<InteractableObject3D>().parent_slot.DropItem();
            }
            
            GetComponent<InteractableObject3D>().Despawn();
        }
    }
}
