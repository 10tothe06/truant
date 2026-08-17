using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// an item, in an inventory

[System.Serializable]
public class inv_itemstack
{
    // the basics, same as older inventory implementations
    public int itemIndex;
    public int itemCount;


    public List<string> data_keys;
    public List<string> data_values;


    public int cellIndex; // where the "origin" of the item is

    // how many cells the item extends in either direction
    // this works with negative too (positive = right and up)
    public int extendHorizontal;
    public int extendVertical;

    public inv_itemstack()
    {
        data_keys = new List<string>();
        data_values = new List<string>();

        extendVertical = 1;
        extendHorizontal = 1;
    }

    public inv_itemstack(string item_name, int itemCount, int cellIndex)
    {
        this.itemIndex = ItemManager.GetItemIndexFromName(item_name);
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;

        data_keys = new List<string>();
        data_values = new List<string>();
    }


    // assuming rotation is 0
    public inv_itemstack(int itemIndex, int itemCount, int cellIndex)
    {
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;

        data_keys = new List<string>();
        data_values = new List<string>();
    }

    public inv_itemstack(int itemIndex, int itemCount, int cellIndex, List<string> data_keys, List<string> data_values)
    {
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;

        this.data_keys = data_keys;
        this.data_values = data_values;
    }

    public void SetItemIndex(int new_index)
    {
        itemIndex = new_index;

        extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;
    }

    #region ITEM DATA

    // and by this i mean the additional key/value pair data that items can have
    

    public void SetData(string key, float new_value)
    {
        SetData(key, new_value.ToString());
    }
    public void SetData(string key, string new_value)
    {
        bool existingEntry = false;

        for (int i = 0; i < data_keys.Count; i++)
        {
            if (data_keys[i] == key)
            {
                data_values[i] = new_value;
                existingEntry = true;
                break;
            }
        }

        if (!existingEntry)
        {
            // making a new entry cuz we didnt find one
            data_keys.Add(key);
            data_values.Add(new_value);
        }
    }

    // similar to how WPILib (the old comms protocol) has built-in parsing function
    public string GetString(string key)
    {
        for (int i = 0; i < data_keys.Count; i++)
        {
            if (data_keys[i] == key)
            {
                return data_values[i];
            }
        }

        // default is empty
        return "";
    }

    public float GetFloat(string key)
    {
        string raw = GetString(key);
        float parsedValue = 0;
        
        if (float.TryParse(raw, out parsedValue))
        {
            return parsedValue;
        }

        // default
        return 0;
    }


    #endregion


    // TODO: actually factor in rotation index
    // public inv_itemstack(int itemIndex, int itemCount, int cellIndex, int rotationIndex)
    // {
    //     this.itemIndex = itemIndex;
    //     this.itemCount = itemCount;

    //     this.cellIndex = cellIndex;

    //     // filling out the extend horizontal and vertical based on the item's static data
    //     this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
    //     this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;
    // }

    public static inv_itemstack ParseFromString(string s)
    {
        inv_itemstack i = new inv_itemstack();

        string[] split = util_string.SplitByChar(s, '#');

        i.itemIndex = int.Parse(split[0]);
        i.itemCount = int.Parse(split[1]);

        i.cellIndex = int.Parse(split[2]);

        i.extendHorizontal = int.Parse(split[3]);
        i.extendVertical = int.Parse(split[4]);

        return i;
    }

    public string FormatAsString()
    {
        string s = "";

        s += itemIndex + "#";
        s += itemCount + "#";

        s += cellIndex + "#";


        s += extendHorizontal + "#";
        s += extendVertical + "#";

        return s;
    }

    public inv_itemstack(inv_itemstack src)
    {
        this.itemIndex = src.itemIndex;
        this.itemCount = src.itemCount;

        this.cellIndex = src.cellIndex;

        this.extendHorizontal = src.extendHorizontal;
        this.extendVertical = src.extendVertical;

        this.data_keys = src.data_keys;
        this.data_values = src.data_values;
    }

    public inv_itemdata GetData()
    {
        return ItemManager.Instance.items[itemIndex];
    }
}
