using UnityEngine.UI;
using UnityEngine;

public class ui_hotbar : MonoBehaviour
{
    public Image i_selectedCell;


    public int selected_cell;

    public Sprite[] cell_borders;

    public void SelectCell(int cell_index)
    {
        selected_cell = cell_index;

        UpdateSelectedCellBorder();
    }

    private void UpdateSelectedCellBorder()
    {
        inv_itemstack selected_item = Player.player_inventory.GetItemTakingUpCell(selected_cell);

        if (selected_item == null)
        {
            Transform t = GetComponent<ui_inventories>().t_inventoryContainer.GetChild(0).GetComponent<ui_inventorywidget>().t_cellContainer.GetChild(selected_cell);

            i_selectedCell.transform.position = t.position + new Vector3(ItemManager.rawInventoryCellSize / 2f, -ItemManager.rawInventoryCellSize / 2f, 0);
            i_selectedCell.GetComponent<RectTransform>().sizeDelta = Vector3.one * (ItemManager.rawInventoryCellSize + 10f);
        } else
        {
            Transform t = GetComponent<ui_inventories>().t_inventoryContainer.GetChild(0).GetComponent<ui_inventorywidget>().t_cellContainer.GetChild(selected_item.cellIndex);

            i_selectedCell.transform.position = t.position + new Vector3(ItemManager.rawInventoryCellSize*selected_item.extendHorizontal / 2f, -ItemManager.rawInventoryCellSize / 2f, 0);
            i_selectedCell.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemManager.rawInventoryCellSize*selected_item.extendHorizontal + 10f, ItemManager.rawInventoryCellSize + 10f);
        }
    }

    public void SelectNextCell(int step)
    {
        if (step == 0) {return;}

        inv_itemstack stack = Player.player_inventory.GetItemTakingUpCell(selected_cell);

        int newIndex = util_math.StepWithinBounds(selected_cell, step, 0, 7);
        inv_itemstack new_stack = Player.player_inventory.GetItemTakingUpCell(newIndex);

        if (new_stack == null)
        {
            SelectCell(newIndex);
            return;
        }

        int safe_iterations = 10;
        int num_iterations = 0;
        while(new_stack == stack && num_iterations < safe_iterations)
        {
            newIndex = util_math.StepWithinBounds(newIndex, step, 0, 7);
            new_stack = Player.player_inventory.GetItemTakingUpCell(newIndex);

            num_iterations++;
        }

        if (new_stack != null)
        {
            SelectCell(new_stack.cellIndex);
        } else
        {
            SelectCell(newIndex);
        }
    }
}
