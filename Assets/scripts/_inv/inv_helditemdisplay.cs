using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// why is this a separate script?
// who the shit knows really

public class inv_helditemdisplay : MonoBehaviour
{
    public float inspect_rotation_speed;

    public Transform t_heldItemContainer;
    public Transform t_inspectingItemPosition;

    public GameObject g_currentlyHeldObject {get; private set;}
    public bool is_inspecting_item {get; private set;}


    // the latest entry in a LONG chain of item-related unity events
    public UnityEvent onUpdateHeldObject;

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

        onUpdateHeldObject.Invoke();
    }

    void Update()
    {
        if (g_currentlyHeldObject != null)
        {
            if (Keyboard.current.leftAltKey.isPressed)
            {
                if (Keyboard.current.leftAltKey.wasPressedThisFrame)
                {
                    if (g_currentlyHeldObject.GetComponent<InteractableObject3D>() != null)
                    {
                        g_currentlyHeldObject.GetComponent<InteractableObject3D>().onInspectObject.Invoke();
                    }

                    is_inspecting_item = true;
                }

                // moving the item
                g_currentlyHeldObject.transform.position = t_inspectingItemPosition.position;
                

                // rotating the item
                Vector3 rot = CameraController.t_cam.right * Input.mouseMovement.y + Vector3.up * -Input.mouseMovement.x;

                g_currentlyHeldObject.transform.Rotate(rot * inspect_rotation_speed * Time.deltaTime, Space.World);

                Player.LockAll();

            } else
            {
                // just holding the item like normal

                if (Keyboard.current.leftAltKey.wasReleasedThisFrame)
                {
                    if (g_currentlyHeldObject.GetComponent<InteractableObject3D>() != null)
                    {
                        g_currentlyHeldObject.GetComponent<InteractableObject3D>().onFinishInspecting.Invoke();
                    }

                    is_inspecting_item = false;

                    // some items handle their position on their own,
                    // so we only want to move the item ONCE and not periodically
                    g_currentlyHeldObject.transform.localPosition = Vector3.zero;
                }

                Player.UnlockAll();
            }
        }
    }
}
