using UnityEngine;

// an item, in an inventory

[System.Serializable]
public class inv_itemstack
{
    // the basics, same as older inventory implementations
    public int itemIndex;
    public int itemCount;




    public int cellIndex; // where the "origin" of the item is

    // how many cells the item extends in either direction
    // this works with negative too (positive = right and up)
    public int extendHorizontal;
    public int extendVertical;

    public inv_itemstack() {}


    // assuming rotation is 0
    public inv_itemstack(int itemIndex, int itemCount, int cellIndex)
    {
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;
    }


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
    }

    public inv_itemdata GetData()
    {
        return ItemManager.Instance.items[itemIndex];
    }
}
