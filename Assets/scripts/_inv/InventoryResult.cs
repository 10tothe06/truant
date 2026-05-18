using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryResult
{
    public ItemStack[] data;
    public int overflow;

    public InventoryResult(ItemStack[] _data, int _overflow)
    {
        data = _data;
        overflow = _overflow;
    }
}
