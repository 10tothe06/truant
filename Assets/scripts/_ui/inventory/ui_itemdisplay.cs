using TMPro;
using UnityEngine;
using UnityEngine.UI;

// standardized class for dealing with the item icon itself, the background, and of course the item count display

public class ui_itemdisplay : MonoBehaviour
{
    // components on this object
    private Image i;
    private RectTransform rt;


    // child components
    public TextMeshProUGUI tx_itemCount;
    public Image i_itemBG;
    public Image i_itemIcon;


    void Awake()
    {
        rt = GetComponent<RectTransform>();
        i = GetComponent<Image>();
    }


    #region DRAW FUNCTIONS


    // these two solve a layering issue in which empty item cells take precedent over filled ones because of how the heirarchy is set up
    // easiest way to solve this is to allow other scripts to hide the image component completely,
    // when this slot is occupied by another item

    // they are wrapper functions so any future logic can be added
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    // no clear function, just pass null in here
    public void Draw(inv_itemstack itemData)
    {
        // could automatically call the show function, 
        // but i'll leave it out in case I run into a case where I need to not

        if (itemData == null) // this is how the display is told to clear itself, passing a null item data
        {
            rt.sizeDelta = new Vector2(ItemManager.rawInventoryCellSize, ItemManager.rawInventoryCellSize);
            i_itemIcon.gameObject.SetActive(false);
            i_itemBG.gameObject.SetActive(false);
            tx_itemCount.gameObject.SetActive(false);
            return;
        } else
        {
            i_itemIcon.gameObject.SetActive(true);
            i_itemBG.gameObject.SetActive(true);
            tx_itemCount.gameObject.SetActive(true);
        }

        // dimensions are set for this object, all child images are set to 'stretch' so they copy them
        rt.sizeDelta = new Vector2(ItemManager.rawInventoryCellSize * itemData.extendHorizontal, ItemManager.rawInventoryCellSize * itemData.extendVertical);

        // the text that shows how many items are in the stack
        tx_itemCount.text = "x" + itemData.itemCount;
        

        // since these two copy their dims, I just need to set the sprite
        i_itemIcon.sprite = itemData.GetData().icon;
        i_itemBG.sprite = ItemManager.GetItemBackground(itemData.extendHorizontal, itemData.extendVertical);
    }

    #endregion
}
