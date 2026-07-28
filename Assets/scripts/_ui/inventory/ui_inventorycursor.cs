using UnityEngine;
using UnityEngine.UI;

public class ui_inventorycursor : MonoBehaviour
{
    [HideInInspector]
    public inv_itemstack heldItem;

    public ui_itemdisplay itemDisplay;

    void Awake()
    {
        heldItem = null;
    }

    public void SetItem(inv_itemstack item)
    {
        heldItem = item;
        DrawHeldItem();
    }
    public void ClearItem()
    {
        heldItem = null;
        DrawHeldItem();
    }

    private void DrawHeldItem()
    {
        if (heldItem == null)
        {
            itemDisplay.Hide();
        } else
        {
            itemDisplay.Show();
        }

        itemDisplay.Draw(heldItem);
    }
}
