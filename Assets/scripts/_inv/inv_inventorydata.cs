using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// better organizing inventory data being sent over the network with this class

// appears on crft_inventory

[System.Serializable]
public class inv_inventorydata
{
    public int inventory_width;
    public int inventory_height;

    // inventory data is stored in 2 parts
    public bool[] cellsTaken;
    public List<inv_itemstack> items;

    public inv_inventorydata(){items = new List<inv_itemstack>();}

    public inv_inventorydata(int inventory_width, int inventory_height)
    {
        this.inventory_width = inventory_width;
        this.inventory_height = inventory_height;
    }

    public void ApplyData(inv_inventorydata i)
    {
        for (int n = 0; n < i.items.Count; n++)
        {
            AddItem(i.items[n]);
        }
    }

    public static inv_inventorydata ParseFromString(string data)
    {
        inv_inventorydata i = new inv_inventorydata();

        string[] items_data = util_string.SplitByChar(data, '&');

        for (int j = 0; j < items_data.Length; j++)
        {
            i.items.Add(inv_itemstack.ParseFromString(items_data[j]));
        }

        return i;
    }


    public string FormatAsString()
    {
        // cells taken can be deduced, and width and height are static
        // so we just have to package up the items list, which shouldn't be too bad


        string data = "";


        for (int i = 0; i < items.Count; i++)
        {
            data += items[i].FormatAsString();
            if (i < items.Count-1)
            {
                data += "&";
            }
        }

        return data;
    }

    // returns a COPY, not the real thing
    public inv_itemstack GetItemAtCell(int cellIndex)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].cellIndex == cellIndex)
            {
                return new inv_itemstack(items[i]);
            }
        }

        return null;
    }
    // returns the ACTUAL REFERENCE
    public inv_itemstack GetItemReferenceAtCell(int cellIndex)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].cellIndex == cellIndex)
            {
                return items[i];
            }
        }

        return null;
    }

    #region CHECKS


    // looking for a quantity of an item type
    // the cell index of 'data' doesn't matter here
    public bool HasItem(inv_itemstack data)
    {
        int sum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemIndex == data.itemIndex)
            {
                sum += items[i].itemCount;
            }
        }

        if (sum >= data.itemCount)
        {
            return true;
        } else
        {
            return false;
        }
    }


    public bool CanFitItem(inv_itemstack data)
    {
        for (int x = 0; x < data.extendHorizontal; x++)
        {
            for (int y = 0; y < data.extendVertical; y++)
            {
                int indexOffset = x + y * inventory_width;

                if (indexOffset + data.cellIndex >= cellsTaken.Length)
                {
                    return false;
                }
                if (cellsTaken[data.cellIndex + indexOffset])
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion

    #region ADDING/REMOVING


    // removing a SPECIFIC ITEM from a cell
    
    // the item count matters here, if the count is less than the number in the cell we only remove that amt
    // of course if the count is the same we remove all of it
    public void RemoveItem(inv_itemstack data)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].cellIndex == data.cellIndex && items[i].itemIndex == data.itemIndex)
            {
                if (data.itemCount >= items[i].itemCount)
                {
                    items.RemoveAt(i);

                    UpdateSelectedCellsForItem(data, false);

                    return; // we're done, so we do this to avoid loop issues
                } else
                {
                    items[i].itemCount -= data.itemCount;
                    
                    return; // same story, we're done
                }
            }
        }
    }

    // adding an item to the inventory
    public void AddItem(inv_itemstack data)
    {
        // there are two possibilities here, either we combine the stack with another of the same type or we add it
        // if neither, then we return
        if (cellsTaken[data.cellIndex])
        {
            if (GetItemAtCell(data.cellIndex).itemIndex == data.itemIndex)
            {
                GetItemReferenceAtCell(data.cellIndex).itemCount = Mathf.Min(GetItemAtCell(data.cellIndex).GetData().stackSize, GetItemAtCell(data.cellIndex).itemCount + data.itemCount);
            } else
            {
                return; // can't add
            }
        } else
        {
            items.Add(data);
        }

        // no need to refresh the whole array here, just need to set the cells for this one item
        UpdateSelectedCellsForItem(data, true);
    }

    public void UpdateSelectedCellsForItem(inv_itemstack data, bool isPlaced) // either placed (set cells to taken) or removed (unset them)
    {
        for (int x = 0; x < data.extendHorizontal; x++)
        {
            for (int y = 0; y < data.extendVertical; y++)
            {
                int indexOffset = x + y * inventory_width;

                cellsTaken[data.cellIndex + indexOffset] = isPlaced;
            }
        }
    }

    #endregion
}
