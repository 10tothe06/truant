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

    public static int_interactionsource interaction_source;

    public static bool isDraggingObject;
    public static bool isCarryingObject;


    #region STATS

    public static float drunk_level;

    #endregion



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

    #region ITEMS

    public static void DropSelectedItem()
    {
        ObjectManager.SpawnObject(GetSelectedItemData().item_name, player_hotbar.GetComponent<inv_helditemdisplay>().t_heldItemContainer.position);

        player_inventory.RemoveItem(GetSelectedItem());

        player_hotbar.Refresh();
        UIManager.Instance.inventory.OpenPlayerInventory();
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

        UIManager.Instance.inventory.OpenPlayerInventory();
    }
}
