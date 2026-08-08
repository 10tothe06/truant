using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class inv_itemdata
{
    public string item_name;
    public List<string> item_tags;

    public int stackSize;

    // items can be rotated, keep in mind
    public int occupyWidth;
    public int occupyHeight;

    public Sprite icon;

    public inv_itemdata() {}

    public inv_itemdata(int occupyWidth, int occupyHeight)
    {
        this.occupyWidth = occupyWidth;
        this.occupyHeight = occupyHeight;
        item_tags = new List<string>();
    }

    public inv_itemdata(string item_name, int occupyWidth, int occupyHeight)
    {
        this.item_name = item_name;

        
        this.occupyWidth = occupyWidth;
        this.occupyHeight = occupyHeight;

        item_tags = new List<string>();
    }
}
