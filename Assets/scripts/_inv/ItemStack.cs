using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// a reference to an item in an inventory
// can be parsed to/from strings so that the data can be carried across the network,
// piggy-backing on top of the existing entity data system

[System.Serializable]
public class ItemStack
{
    // what type of item we're referencing
    // (references a central item array)

    // we're handling "custom" items like metal alloys and such by just adding to the central item array
    // if we put everything inside of keys/values it would suck real bad
    public int id;


    // how much of the item we have in a stack
    public int count;

    // extra data for the item, stuff that is stored on a per-item basis
    // (for example, durability)
    public List<string> keys;
    public List<string> values;

    public string GetName()
    {
        if (keys.Contains("custom name"))
        {
            return values[keys.IndexOf("custom name")];
        }
        else
        {
            return WorldData.Instance.items[id].name;
        }
    }

    public ItemStack() {}
    public ItemStack(string name, int count)
    {
        int index = -1;
        for (int i = 0; i < WorldData.Instance.items.Length; i++)
        {
            if (WorldData.Instance.items[i].name == name)
            {
                index = i;
                break;
            }
        }
        if (index == -1) return;

        this.id = index;
        this.count = count;

        this.keys = new List<string>();
        this.values = new List<string>();
    }

    public ItemStack(int id, int count)
    {
        this.id = id;
        this.count = count;

        this.keys = new List<string>();
        this.values = new List<string>();
    }

    // the full constructor, nothing assumed/empty
    public ItemStack(int id, int count, List<string> keys, List<string> values)
    {
        this.id = id;
        this.count = count;

        this.keys = keys;
        this.values = values;
    }

    public string GetValueAt(string key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                return values[i];
            }
        }

        return "";
    }

    public void SetValueAt(string key, string newValue)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                values[i] = newValue;
                return;
            }
        }

        keys.Add(key);
        values.Add(newValue);
    }

    public Item GetClass()
    {
        return WorldData.Instance.items[id];
    }

    // *********************************************
    // Parsers
    // *********************************************

    public static ItemStack[] ParseArrayFromString(string str)
    {
        string[] splitByCommas = util_string.SplitByChar(str,',');

        ItemStack[] result = new ItemStack[splitByCommas.Length];
        
        for (int i = 0; i < splitByCommas.Length; i++)
        {
            result[i] = ParseFromString(splitByCommas[i]);
        }

        return result;
    }

    public static ItemStack ParseFromString(string str)
    {
        ItemStack result = new ItemStack();

        string[] split = util_string.SplitByChar(str, '|');

        result.id = int.Parse(split[0]);
        result.count = int.Parse(split[1]);

        string[] splitKeys = util_string.SplitByChar(split[2], '/');

        result.keys = new List<string>();
        result.values = new List<string>();

        for (int i = 0; i < splitKeys.Length; i++)
        {
            string[] keyValueSplit = util_string.SplitByChar(splitKeys[i],':'); // TODO: use the substring function cuz its less heavy

            result.keys.Add(keyValueSplit[0]);
            result.values.Add(keyValueSplit[1]);
        }

        return result;
    }


    
    public static string ParseArrayToString(ItemStack[] items)
    {
        string result = "";

        for (int i = 0; i < items.Length; i++)
        {
            result += items[i].ParseToString();
            if (i < items.Length - 1)
            {
                result += ",";
            }
        }

        return result;
    }
    // no commas are used here, so we can more easily make arrays out of these
    public string ParseToString()
    {
        string result = "";

        result += id;
        result += "|";
        result += count;
        result += "|";

        // keys and values are separated by : characters
        // entries are separated by / characters

        for (int i = 0; i < keys.Count; i++)
        {
            result += keys[i];
            result += ":";
            result += values[i];

            if (i < keys.Count - 1)
            {
                result += "/";
            }
        }

        return result;
    }

    // ******************************************
    // functions that mess with ItemStack arrays
    // ******************************************

    public static int CheckForItem(int type, ItemStack[] data)
    {
        int itemCount = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == null) {continue;}
            if (data[i].id == type)
            {
                itemCount += data[i].count;
            }
        }

        return itemCount;
    }

    public static void RemoveItem(int type, int amount, ItemStack[] data) {

        int amountLeft = amount;

        for (int i = 0; i < data.Length; i++)
        {
            if (amountLeft == 0) { break; }

            if (data[i] != null)
            {
                if (data[i].id == type)
                {
                    int space = data[i].count;

                    if (amountLeft < space)
                    {
                        data[i].count -= amountLeft;
                        amountLeft = 0;
                    }
                    else
                    {
                        amountLeft -= space;
                        data[i] = null;
                    }
                }
            }
        }
    }
    // removing 1 of an item
    public static void RemoveItem(int type, ItemStack[] data) {
        RemoveItem(type, 1, data);
    }

    public static InventoryResult AddItem(int type, int amount, ItemStack[] data) {
        return AddItem(type, amount, data, new List<string>(), new List<string>());
    }

    public static InventoryResult AddItem(int type, int amount, ItemStack[] data, List<string> keys, List<string> values)
    {
        List<int> containedItems = new List<int>();

        List<string> newKeys = new List<string>();
        List<string> newValues = new List<string>();

        for (int i = 0; i < keys.Count; i++)
        {
            newKeys.Add(keys[i]);
        }

        for (int i = 0; i < values.Count; i++)
        {
            newValues.Add(values[i]);
        }

        ItemStack[] output = data;
        int amountLeft = amount;

        for (int i = 0; i < output.Length; i++)
        {
            if (amountLeft == 0) { break; }

            if (output[i] != null)
            {
                if (output[i].id == type)
                {
                    int space = WorldData.Instance.items[output[i].id].stackSize - output[i].count;

                    if (amountLeft <= space)
                    {
                        data[i].count += amountLeft;
                        amountLeft = 0;
                    }
                    else
                    {
                        amountLeft -= space;
                        data[i].count += space;
                    }
                }
            }
            else
            {
                int space = WorldData.Instance.items[type].stackSize;

                if (amountLeft <= space)
                {
                    data[i] = new ItemStack(type, amountLeft, newKeys, newValues);
                    amountLeft = 0;
                }
                else
                {
                    data[i] = new ItemStack(type, space, newKeys, newValues);
                    amountLeft -= space;
                }
            }
        }

        //I hate returning a custom class like this instead of just a stack array, but I need to pass on the amount of items left in case they can't all fit
        return new InventoryResult(output, amountLeft);
    }
}
