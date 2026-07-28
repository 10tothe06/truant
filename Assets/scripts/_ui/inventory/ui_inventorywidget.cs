using System;
using UnityEngine;
using UnityEngine.Events;

// this is used when actually displaying an inventory


// this script is NOT featured on craft parts, 
// because their inventory only needs to be displayed when you "open" them
// in which case copies of this class are created as necessary

public class ui_inventorywidget : MonoBehaviour
{
    public bool drawCentered;


    public Func<inv_inventorydata> source;
    //[HideInInspector]
    public inv_inventorydata cachedSourceData;

    public Transform t_cellContainer;

    public GameObject p_cell;

    public Transform t_itemIconContainer;

    [HideInInspector]
    public float effectiveHeight;

    //public float spaceBetweenCells;

    public void BuildMenu(Func<inv_inventorydata> source, Vector3 position)
    {
        transform.position = position;

        this.source = source;
        UpdateCachedData();


        BuildCellGrid();

        DrawItems();
    }

    private void DrawItems()
    {
        for (int i = 0; i < cachedSourceData.items.Count; i++)
        {
            t_cellContainer.GetChild(cachedSourceData.items[i].cellIndex).GetComponent<ui_inventoryslot>().SetItem(cachedSourceData.items[i]);
        }
    }

    private void BuildCellGrid()
    {
        float spaceBetweenCells = 0; // has to be zero, really
        float cellSize = 100f; // can just use x or y cuz they're square

        Vector3 positionOffset = Vector3.zero;
        if (drawCentered)
        {
            positionOffset = new Vector3(-cachedSourceData.inventory_width / 2f * cellSize - (cachedSourceData.inventory_width-1)/ 2f * spaceBetweenCells, cachedSourceData.inventory_height / 2f * cellSize + (cachedSourceData.inventory_height-1)/ 2f * spaceBetweenCells, 0);
        }

        for (int y = 0, i = 0; y < cachedSourceData.inventory_height; y++)
        {
            for (int x = 0; x < cachedSourceData.inventory_width; x++, i++)
            {
                GameObject g_newCell = Instantiate(p_cell, t_cellContainer);

                // positioning the cell
                g_newCell.transform.localPosition = positionOffset + new Vector3(x * (cellSize + spaceBetweenCells), -y * (cellSize + spaceBetweenCells), 0);
                g_newCell.GetComponent<ui_inventoryslot>().Initialize(t_itemIconContainer, this, i);
            }
        }

        // no space between cells
        effectiveHeight = cachedSourceData.inventory_height * cellSize;
    }

    void UpdateCachedData()
    {
        cachedSourceData = source.Invoke();
    }

    void RefreshWidget()
    {
        for (int i = 0; i < t_itemIconContainer.childCount; i++)
        {
            t_cellContainer.GetChild(i).GetComponent<ui_inventoryslot>().SetItem(null);
        }

        DrawItems();
    }



    // if not right click, then left click
    public void HandleInteractionAtCell(int cellIndex, bool isRightClick)
    {
        Debug.Log("handling interation with cell " + cellIndex + "...");
        // remember, we only update the data once an item has been PLACED, not removed
        // and the update is done through  an inv_inventorytransfer, NEVER here

        if (cachedSourceData.cellsTaken[cellIndex])
        {
            
            if (ui_inventories.Instance.IsHoldingItem())
            {
                if (isRightClick)
                {
                    // needs to be the same type to combine stacks
                    if (ui_inventories.Instance.cursor.heldItem.itemIndex == cachedSourceData.GetItemAtCell(cellIndex).itemIndex)
                    {
                        inv_itemstack data = new inv_itemstack(ui_inventories.Instance.cursor.heldItem);
                        data.itemCount = 1;
                        data.cellIndex = cellIndex;

                        inv_itemstack newCursorData = new inv_itemstack(ui_inventories.Instance.cursor.heldItem);
                        newCursorData.itemCount -= 1;

                        if (newCursorData.itemCount > 0)
                        {
                            ui_inventories.Instance.GiveItemToCursor(newCursorData);
                        } else
                        {
                            ui_inventories.Instance.ClearCursor();
                        }

                        // add the item to the inventory
                        cachedSourceData.AddItem(data);

                        RefreshWidget();
                    }
                } else
                {
                    // needs to be the same type to combine stacks
                    if (ui_inventories.Instance.cursor.heldItem.itemIndex == cachedSourceData.GetItemAtCell(cellIndex).itemIndex)
                    {
                        inv_itemstack data = ui_inventories.Instance.cursor.heldItem;
                        data.cellIndex = cellIndex;

                        // remove the held item stack from the cursor
                        ui_inventories.Instance.ClearCursor();
                        // add the item to the inventory
                        cachedSourceData.AddItem(data);

                        RefreshWidget();
                    }
                }
            } else
            {
                
                if (isRightClick)
                {
                    inv_itemstack data = cachedSourceData.GetItemAtCell(cellIndex);
                    data.itemCount = Mathf.CeilToInt((float)data.itemCount / 2f);

                    ui_inventories.Instance.GiveItemToCursor(data);
                    cachedSourceData.RemoveItem(data);

                    RefreshWidget();
                } else
                {
                    
                    inv_itemstack data = cachedSourceData.GetItemAtCell(cellIndex);
                    ui_inventories.Instance.GiveItemToCursor(data);
                    cachedSourceData.RemoveItem(data);

                    RefreshWidget();
                }
            }



        } else
        {
            if (ui_inventories.Instance.IsHoldingItem())
            {
                if (isRightClick)
                {
                    inv_itemstack data = new inv_itemstack(ui_inventories.Instance.cursor.heldItem);
                    data.itemCount = 1;
                    data.cellIndex = cellIndex;

                    if (!cachedSourceData.CanFitItem(data))
                    {
                        return;
                    }

                    inv_itemstack newCursorData = new inv_itemstack(ui_inventories.Instance.cursor.heldItem);
                    newCursorData.itemCount -= 1;

                    if (newCursorData.itemCount > 0)
                    {
                        ui_inventories.Instance.GiveItemToCursor(newCursorData);
                    } else
                    {
                        ui_inventories.Instance.ClearCursor();
                    }

                    // add the item to the inventory
                    cachedSourceData.AddItem(data);

                    RefreshWidget();
                } else
                {
                    inv_itemstack data = ui_inventories.Instance.cursor.heldItem;
                    data.cellIndex = cellIndex;

                    // before we do anything, we need to make sure that placing an item here will not cause any overlaps
                    // we do this by checking the cellsTaken[] array
                    if (!cachedSourceData.CanFitItem(data))
                    {
                        return;
                    }

                    // remove the held item stack from the cursor
                    ui_inventories.Instance.ClearCursor();
                    // add the item to the inventory
                    cachedSourceData.AddItem(data);

                    RefreshWidget();
                }
            } else
            {
                if (isRightClick)
                {
                    // nothing happens
                } else
                {
                    // nothing happens
                }
            }
        }
    }
}
