using UnityEngine.UI;
using UnityEngine;

public class ui_inventoryslot : MonoBehaviour
{
    private inv_itemstack cachedItemData;
    private RectTransform rt;



    public GameObject p_itemIcon;
    private ui_itemdisplay itemDisplay;

    private int cellIndex;
    private ui_inventorywidget parentWidget;



    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(Transform t_itemIconContainer, ui_inventorywidget parentWidget, int cellIndex)
    {
        this.parentWidget = parentWidget;
        this.cellIndex = cellIndex;


        GameObject g_itemIcon = Instantiate(p_itemIcon, t_itemIconContainer);

        g_itemIcon.transform.position = transform.position;

        itemDisplay = g_itemIcon.GetComponent<ui_itemdisplay>();
        itemDisplay.Draw(null);
        UpdateHiddenState();

        itemDisplay.GetComponent<ui_button>().onPressSpecific.AddListener((x) => HandleInteractionWithSlot(x));
        itemDisplay.GetComponent<ui_button>().onDragSpecific.AddListener((x) => HandleInteractionWithSlot(x));
    }

    // THERE IS NO CLEAR ITEM FUNCTION, JUST PASS 'null' IN HERE
    public void SetItem(inv_itemstack data)
    {
        // update the cache
        cachedItemData = data;

        // literally everything else is handled by ui_itemdisplay
        itemDisplay.Draw(data);

        UpdateHiddenState();
    }

    private void UpdateHiddenState()
    {
        if (parentWidget.cachedSourceData.cellsTaken[cellIndex])
        {
            if (cachedItemData != null)
            {
                itemDisplay.Show();
            } else
            {
                itemDisplay.Hide();
            }
        } else
        {
            itemDisplay.Show();
        }
    }

    public void HandleInteractionWithSlot(ushort clickType)
    {
        parentWidget.HandleInteractionAtCell(cellIndex, clickType == 1);
    }
}
