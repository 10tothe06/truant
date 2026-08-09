using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    // this is used for most things, static functions can also be used when verbosity is a concern
    public static Player Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        // assinging components that can be accessed by other scripts
        rb = GetComponent<Rigidbody>();
        controller= GetComponent<PlayerController>();
        generic_controller = GetComponent<player_genericcontroller>();
        interaction_source = GetComponent<int_interactionsource>();
        t = transform;

        player_hotbar = ins_player_hotbar;

        item_holder = player_hotbar.GetComponent<inv_helditemdisplay>();
    }

    void Start()
    {
        player_inventory = new inv_inventorydata(8, 1);

        // adding to the debug menu
        ui_debugmenu.AddEntry("player_pos", () => transform.position.ToString());
    }

    public static Transform t;
    public static PlayerController controller;
    public static player_genericcontroller generic_controller;
    public static Rigidbody rb;


    public static inv_inventorydata player_inventory;

    public ui_hotbar ins_player_hotbar;
    public static ui_hotbar player_hotbar;
    public static inv_helditemdisplay item_holder;

    public static int_interactionsource interaction_source;

    public static bool isDraggingObject;
    public static bool isCarryingObject;


    #region STATS

    public static float drunk_level;
    // must modify through the ModifyHealth() function
    public static float health {get; private set;}

    #endregion


    public static void ModifyHealth(float amt)
    {
        health += amt;
    }

    

    #region LOCKING

    public static void LockCamera()
    {
        // freeze player movement and looking
        PlayerController comp = Player.controller;

        comp.lockCameraHorizontal = true;
        comp.lockCameraVertical = true;
    }

    public static void LockAll()
    {
        // freeze player movement and looking
        PlayerController comp = Player.controller;

        comp.lockCameraHorizontal = true;
        comp.lockCameraVertical = true;
        comp.lockMovement = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public static void UnlockAll()
    {
        // freeze player movement and looking
        PlayerController comp = Player.controller;

        comp.lockCameraHorizontal = false;
        comp.lockCameraVertical = false;
        comp.lockMovement = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    # endregion

    public static GameObject GetHeldObject()
    {
        return item_holder.g_currentlyHeldObject;
    }

    #region ITEMS


    // returns the object that was dropped
    public static GameObject DropSelectedItem()
    {
        if (GetSelectedItemData() == null) {return null;}

        
        GameObject g = ObjectManager.SpawnObject(GetSelectedItemData().item_name, player_hotbar.GetComponent<inv_helditemdisplay>().t_heldItemContainer.position);


        // all items should have this comp,
        // but it IN THEORY might not so this is to prevent an error
        if (g.GetComponent<int_item>())
        {
            g.GetComponent<int_item>().SetItemData(new inv_itemstack(GetSelectedItem()));
        } else
        {
            Debug.LogWarning("item prefab does not have item component?");
        }

        player_inventory.RemoveItem(GetSelectedItem());

        // the hotbar and the inventory will automatically rebuild themselves,
        // to show the change (bc of the RemoveItem() call)

        return g;
    }

    public static inv_itemdata GetSelectedItemData()
    {
        if (player_inventory.GetItemAtCell(player_hotbar.selected_cell) == null) {return null;}
        return player_inventory.GetItemAtCell(player_hotbar.selected_cell).GetData();
    }
    public static inv_itemstack GetSelectedItem()
    {
        return player_inventory.GetItemAtCell(player_hotbar.selected_cell);
    }

    #endregion

    public static void TeleportTo(Vector3 position)
    {
        Instance.transform.position = position;
    }

    public static void GiveItem(inv_itemstack data)
    {
        player_inventory.AddItem(data);

        // hotbar and inventory will auto-rebuild after we call this function
    }
}
