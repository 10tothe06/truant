using UnityEngine;


[System.Serializable]
public class inv_itemdata
{
    public string item_name;

    public int stackSize;

    // items can be rotated, keep in mind
    public int occupyWidth;
    public int occupyHeight;

    public Sprite icon;

    public string associated_object;

    public inv_itemdata() {}

    public inv_itemdata(int occupyWidth, int occupyHeight)
    {
        this.occupyWidth = occupyWidth;
        this.occupyHeight = occupyHeight;
    }

    public inv_itemdata(string item_name, int occupyWidth, int occupyHeight)
    {
        this.item_name = item_name;

        
        this.occupyWidth = occupyWidth;
        this.occupyHeight = occupyHeight;
    }
}
