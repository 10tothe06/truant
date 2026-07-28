using System.Collections.Generic;
using UnityEngine;

// singleton script that stores static data for all the items in the game,
// as well as their prefabs (if they're not spacecraft parts)

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;

    public static ItemManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        rawInventoryCellSize = ins_rawInventoryCellSize;
    }


    public inv_itemdata[] items;


    // for the items that are NOT spacecraft parts
    public GameObject[] p_items;

    

    public inv_itembackground[] item_bgs;
    

    public float ins_rawInventoryCellSize;
    public static float rawInventoryCellSize;

    #region ITEMS


    public static int GetItemIndexFromName(string itemName)
    {
        for (int i = 0; i < Instance.items.Length; i++)
        {
            if (Instance.items[i].item_name == itemName)
            {
                return i;
            }
        }

        return -1; // ideally we never get here, but if we do we'll know cuz it will likely throw an error
    }


    public static Sprite GetItemBackground(int width, int height)
    {
        for (int i = 0; i < Instance.item_bgs.Length; i++)
        {
            if (Instance.item_bgs[i].width == width && Instance.item_bgs[i].height == height)
            {
                return Instance.item_bgs[i].img;
            }
        }

        return Instance.item_bgs[0].img; // should never get here
    }


    #endregion
}
