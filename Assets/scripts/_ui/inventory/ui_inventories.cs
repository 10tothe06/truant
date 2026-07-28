using System;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

// plural, because you'll have many crates on you in addition to whatever you're interacting with
// so we have to deal with lots of inventories at the same time

public class ui_inventories : MonoBehaviour
{   
    private static ui_inventories _instance;

    public static ui_inventories Instance
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
        ClearCursor();
    }

    public Transform t_inventoryContainer;
    public GameObject p_inventory;

    public ui_inventorycursor cursor;

    public float spacingBetweenInventories;

    public void BuildMenus(Func<inv_inventorydata>[] sources)
    {
        Vector3 v = new Vector3(Screen.width / 2f, 100f, 0);

        for (int i = 0; i < sources.Length; i++)
        {
            if (i > 0)
            {
                v += Vector3.up * (t_inventoryContainer.GetChild(i-1).GetComponent<ui_inventorywidget>().effectiveHeight/2f);
            }

            GameObject g_newWidget = Instantiate(p_inventory, t_inventoryContainer);

            g_newWidget.GetComponent<ui_inventorywidget>().BuildMenu(sources[i], v);

            v += Vector3.up * (g_newWidget.GetComponent<ui_inventorywidget>().effectiveHeight/2f + spacingBetweenInventories);

            g_newWidget.transform.position = v;
        }
    }

    public void OpenPlayerInventory()
    {
        Func<inv_inventorydata>[] playerInventorySources = new Func<inv_inventorydata>[] {() => Player.player_inventory};

        BuildMenus(playerInventorySources);
    }

    public void OpenExternalInventory(Func<inv_inventorydata> source)
    {
        // List<Func<inv_inventorydata>> toBuild = new List<Func<inv_inventorydata>>();

        // // first we add the player inventories
        // Func<inv_inventorydata>[] playerInventorySources = LocalPlayer.localClient.controllingEntity.GetComponent<player_partmanager>().GetInventorySources();

        // for (int i = 0; i < playerInventorySources.Length; i++)
        // {
        //     toBuild.Add(playerInventorySources[i]);
        // }

        // // finally we add the external inventory (crate or whatever)
        // toBuild.Add(source);

        // // then we can build all the menus
        // BuildMenus(toBuild.ToArray());
    }

    public void ClearMenus()
    {
        for (int i = t_inventoryContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(t_inventoryContainer.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        cursor.transform.position = Input.mousePosition + new Vector2(-ItemManager.rawInventoryCellSize / 2f, ItemManager.rawInventoryCellSize / 2f);
    }

    // ***
    // item taking/giving 
    // ***

    public bool IsHoldingItem()
    {
        return cursor.heldItem != null;
    }

    public void ClearCursor()
    {
        cursor.ClearItem();
    }
    public void GiveItemToCursor(inv_itemstack itemData)
    {
        cursor.SetItem(itemData);
    }
}
