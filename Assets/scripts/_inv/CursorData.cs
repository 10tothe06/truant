using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// data for an item that is being held by the cursor
// we MUST keep track of this, because all inventory scripts expect it and use it to allow you to move items


public class CursorData : MonoBehaviour
{
    private static CursorData _instance;

    public static CursorData Instance
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

        heldItem = null;
    }

    public ItemStack heldItem;

    void Update()
    {
        //toggle the sprite for the held item based on whether there is one
        if (heldItem != null)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().sprite = WorldData.Instance.items[heldItem.id].icon;
                transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        transform.GetChild(0).position = Input.mousePosition;
    }

    public void GiveItem(ItemStack itemToGive) {
        heldItem = itemToGive;
    }

    public void Clear() {
        heldItem = null;
    }
}
