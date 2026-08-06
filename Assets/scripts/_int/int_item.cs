using UnityEngine;

public class int_item : MonoBehaviour
{
    public int itemType;


    void Awake()
    {
        GetComponent<InteractableObject3D>().onInteract.AddListener(Pickup);
    }

    public void Pickup()
    {
        inv_itemstack data = new inv_itemstack(itemType, 1, 0);

        bool canFitItem = false;


        // figuring out whether the player has the inventory space for this item
        for (int i = 0; i < Player.player_inventory.cellsTaken.Length; i++)
        {
            data.cellIndex = i;
            if (Player.player_inventory.CanFitItem(data))
            {
                canFitItem = true;
                break;
            }
        }

        if (canFitItem)
        {
            Player.GiveItem(data);
            GetComponent<InteractableObject3D>().Despawn();
        }
    }
}
