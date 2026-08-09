using UnityEngine;

// why is this a separate script?
// who the shit knows really

public class inv_helditemdisplay : MonoBehaviour
{
    public Transform t_heldItemContainer;
    public GameObject g_currentlyHeldObject {get; private set;}

    void Start()
    {
        // this updates it when the player selects a new item
        GetComponent<ui_hotbar>().onUpdateSelectedItem.AddListener(OnUpdateHeldItem);

        // and this does when the players inventory data changes,
        // such as when they drop an item
        Player.player_inventory.onInventoryUpdate.AddListener(OnUpdateHeldItem);
    }

    private void OnUpdateHeldItem()
    {
        util_canvas.DestroyChildren(t_heldItemContainer.gameObject);

        if (ObjectManager.GetItemObject(Player.GetSelectedItemData()) == null) {g_currentlyHeldObject = null; return;}

        GameObject g_itemDisplay = Instantiate(ObjectManager.GetItemObject(Player.GetSelectedItemData()), t_heldItemContainer);
        if (g_itemDisplay.GetComponent<int_item>())
        {
            g_itemDisplay.GetComponent<int_item>().SetItemData(new inv_itemstack(Player.GetSelectedItem()));
        } else
        {
            Debug.LogWarning("item prefab does not have item component?");
        }

        g_currentlyHeldObject = g_itemDisplay;

        g_itemDisplay.GetComponent<InteractableObject3D>().DisableAllColliders();
        g_itemDisplay.GetComponent<InteractableObject3D>().DisablePhysics();
    }
}
