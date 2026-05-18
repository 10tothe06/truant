using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if we consider ItemStack.cs to be "instanced" data, 
// then we can consider this to be "static" data

[System.Serializable]
public class Item
{
    // name of the item (duh)
    public string name;
    // how many can fit in one inventory slot
    public int stackSize;

    // the item icon seen in inventories
    public Sprite icon;

    // we used to define a mesh that the item used when the player was holding it
    // I felt at the time that this solution worked better since the player would be swapping items quickly

    // I have since changed my mind, and now what happens is the exact same object prefab gets initialized
    // for performance though, it won't be destroyed and will just be disabled when not being held so it doesn't show

    // the index of the object that spawns when the player drops the item
    public GameObject p_item;
    
    // item tags, used to identify weapons, consumables, and such
    public string[] tags;

    public Item() {}
}
