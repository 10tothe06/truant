using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// this is really the most important script in the inventory system
// it goes on any UI assets that need inventory capability, and handles item interaction


public class Inventory : MonoBehaviour
{
    //[HideInInspector]
    public ItemStack[] items; // the items in the inventory
    public Transform element; //the UI element that corresponds to the inventory

    public bool active;
    public bool debugSelectedCell;
    public Sprite emptySprite;

    [Space(6)]
    [Header("Hotbar Data")]
    public bool isHotbar;
    public int selectedCell;
    public UnityEvent onCellSelect;

    [Space(6)]
    [Header("Inventory Dimensions")]
    public int length;
    public int width;
    public bool vertical;
    public float spacing;

    [Space(6)]
    [Header("Item Source Data")]

    public GameObject slotPrefab;

    private bool isSelectionLocked;

    public void Initialize()
    {
        Build();
        Refresh();

        selectedCell = 0;
    }

    public bool HasEmptyCell() // returns true if at least one cell is empty
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    public void Lock()
    {
        isSelectionLocked = true;
    }
    public void Unlock() {
        isSelectionLocked = false;
    }

    // delete all items from the inventory
    public void Clear()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null;
            Refresh();
        }
    }

    void Update()
    {
        if (active)
        {
            if (Input.mouseButtonDownLeft || Input.mouseButtonDownRight)
            {
                HandleInteract();
            }

            if (isHotbar && element.childCount == length * width && !isSelectionLocked)
            {
                for (int i = 0; i <= Mathf.Min(length * width, 9); i++)
                {
                    int selected = -1;
                    if (i == 0 && Keyboard.current.digit0Key.wasPressedThisFrame && length * width >= 10)
                    {
                        selected = 10;
                    } else if (i == 1 && Keyboard.current.digit1Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    }
                    else if (i == 2 && Keyboard.current.digit2Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 3 && Keyboard.current.digit3Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 4 && Keyboard.current.digit4Key.wasPressedThisFrame)
                    {
                    selected = i-1;
                    } else if (i == 5 && Keyboard.current.digit5Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 6 && Keyboard.current.digit6Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 7 && Keyboard.current.digit7Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 8 && Keyboard.current.digit8Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    } else if (i == 9 && Keyboard.current.digit9Key.wasPressedThisFrame)
                    {
                        selected = i-1;
                    }

                    if (selected != -1)
                    {
                        selectedCell = selected;
                        if (items[selectedCell] != null)
                        {
                            onCellSelect.Invoke();
                        }
                    }
                }

                if (debugSelectedCell)
                {
                    for (int i = 0; i < length * width; i++)
                    {
                        if (i == selectedCell)
                        {
                            element.GetChild(i).gameObject.GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            element.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
                        }
                    }
                }
            }
        }
    }

    public ItemStack GetHeldItemStack() {
        // no such thing as a held item if it's not a hotbar
        if (!isHotbar || items[selectedCell] == null) {return null;}

        return items[selectedCell];
    }

    public int GetHeldItemIndex() {
        // no such thing as a held item if it's not a hotbar
        if (!isHotbar || items[selectedCell] == null) {return -1;}

        return items[selectedCell].id;
    }

    public Item GetHeldItemClass() {
        // no such thing as a held item if it's not a hotbar
        if (!isHotbar || items[selectedCell] == null) {return null;}

        return WorldData.Instance.items[GetHeldItemIndex()];
    }

    public void Build()
    {
        items = new ItemStack[length * width];
        for (int i = 0; i < length * width; i++)
        {
            items[i] = null;
        }

        for (int i = 0; i < length * width; i++)
        {
            int x = i % length;
            int y = Mathf.FloorToInt((float)i / (float)length);

            GameObject newSlot = null;
            if (!vertical) { newSlot = Instantiate(slotPrefab, transform.position + new Vector3(1, 0, 0) * spacing * x+ new Vector3(0, 1, 0) * spacing * y, Quaternion.identity); }
            if (vertical) { newSlot = Instantiate(slotPrefab, transform.position + new Vector3(0, 1, 0) * spacing * x + new Vector3(1, 0, 0) * spacing * y, Quaternion.identity); }

            Vector2 size = newSlot.GetComponent<RectTransform>().sizeDelta / 2;
            newSlot.transform.position += new Vector3(size.x, size.y, 0);

            newSlot.name = "slot_" + i.ToString();
            newSlot.transform.SetParent(element);
        }
    }

    public void RemoveHeldItem() {
        if (!isHotbar) {return;}

        items[selectedCell] = null;
        Refresh();
    }

    public void TakeHeldItem() {
        if (!isHotbar) {return;}

        if (items[selectedCell].count > 1) {
            items[selectedCell].count--;
            Refresh();  
        } else {
            items[selectedCell] = null;
            Refresh();
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < length * width; i++)
        {
            if (items[i] != null)
            {
                element.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().sprite = WorldData.Instance.items[items[i].id].icon;
            }
            else
            {
                element.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().sprite = emptySprite;
            }
        }
    }

    void HandleInteract()
    {
        ItemStack[] handleItems = items;
        ItemStack heldCell = null;
        if (CursorData.Instance.heldItem != null) { heldCell = CursorData.Instance.heldItem; }

        for (int i = 0; i < length * width; i++)
        {
            if (element.childCount < length * width) { break; }
            RectTransform currentCell = element.GetChild(i).gameObject.GetComponent<RectTransform>();

            if (Input.mousePosition.x > currentCell.position.x + -currentCell.sizeDelta.x / 2 && Input.mousePosition.x < currentCell.position.x + currentCell.sizeDelta.x / 2)
            {
                if (Input.mousePosition.y > currentCell.position.y + -currentCell.sizeDelta.y / 2 && Input.mousePosition.y < currentCell.position.y + currentCell.sizeDelta.y / 2)
                {
                    //left click
                    if (Input.mouseButtonDownLeft)
                    {
                        if (handleItems[i] == null && heldCell != null) //placing a full stack into an empty cell
                        {
                            handleItems[i] = new ItemStack(heldCell.id, heldCell.count, heldCell.keys, heldCell.values);

                            heldCell = null;
                        }
                        else if (handleItems[i] != null) //taking a full stack from a cell
                        {
                            if (heldCell == null)
                            {
                                heldCell = new ItemStack(handleItems[i].id, handleItems[i].count, handleItems[i].keys, handleItems[i].values);
                                handleItems[i] = null;
                            }
                            else if (heldCell.id == handleItems[i].id)
                            {
                                if (heldCell.count + handleItems[i].count <= WorldData.Instance.items[handleItems[i].id].stackSize) //depositing items from the held cell into another cell
                                {
                                    handleItems[i].count += heldCell.count;
                                    heldCell = null;
                                }
                                else //depositing some items from the held cell AND KEEPING THE REMAINDER IN THE HELD CELL
                                {
                                    heldCell.count -= WorldData.Instance.items[handleItems[i].id].stackSize - handleItems[i].count;
                                    handleItems[i].count = WorldData.Instance.items[handleItems[i].id].stackSize;
                                }
                            }
                        }
                    } // right click
                    else if (Input.mouseButtonDownRight)
                    {
                        if (handleItems[i] == null && heldCell != null) //placing a single item into an empty cell
                        {
                            handleItems[i] = new ItemStack(heldCell.id, 1, heldCell.keys, heldCell.values);
                            heldCell.count -= 1;

                            if (heldCell.count < 1) //make sure to clear the held cell if it has no more items left (we wouldn't want any negative items)
                            {
                                heldCell = null;
                            }
                        }
                        else if (handleItems[i] != null) //taking half of a stack from an empty cell
                        {
                            if (heldCell == null)
                            {
                                if (handleItems[i].count > 1) //you can only take half of a stack if the stack has more than 1 item
                                {
                                    int amountToTake = Mathf.RoundToInt(handleItems[i].count / 2);
                                    heldCell = new ItemStack(handleItems[i].id, amountToTake, handleItems[i].keys, handleItems[i].values);

                                    handleItems[i].count -= amountToTake;
                                }
                                else //if the stack is just 1 item, then take the whole stack (since it cannot be split)
                                {
                                    heldCell = new ItemStack(handleItems[i].id, handleItems[i].count, handleItems[i].keys, handleItems[i].values);
                                    handleItems[i] = null;
                                }
                            }
                            else if (heldCell.id == handleItems[i].id)
                            {
                                if (handleItems[i].count < WorldData.Instance.items[handleItems[i].id].stackSize)
                                {
                                    handleItems[i].count += 1;
                                    heldCell.count -= 1;

                                    if (heldCell.count < 1) //make sure to clear the held cell if it has no more items left (we wouldn't want any negative items)
                                    {
                                        heldCell = null;
                                    }
                                }
                            }
                        }
                    }

                    items = handleItems; 
                    Refresh();
                    break;
                }
            }
        }

        if (heldCell == null)
        {
            CursorData.Instance.GiveItem(null);
        }
        else
        {
            CursorData.Instance.GiveItem(heldCell);
        }
    }

    public bool CheckItemSlot(int slotId, int itemId) {
        if (items[slotId] != null) {
            if (items[slotId].id == itemId) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    // these functions extend the functions from ItemStack.cs, mostly to add a 'Refresh()' call

    public int CheckForItem(int type)
    {
        return ItemStack.CheckForItem(type, items);
    }

    public void RemoveItem(int type, int amount)
    {
        ItemStack.RemoveItem(type, amount, items);
        Refresh();
    }

    public void AddItem(int type, int amount)
    {
        items = ItemStack.AddItem(type, amount, items).data;
        Refresh();
    }
}
