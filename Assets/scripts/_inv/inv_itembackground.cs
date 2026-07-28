using UnityEngine;

// for now, I'm storing all item backgrounds as separate textures
// this system is weird, I'll implement a tiling system later if I need to

[System.Serializable]
public class inv_itembackground
{
    public int width;
    public int height;

    public Sprite img;

    public inv_itembackground() {}
}
