using UnityEngine;


// controller of the advancement window-thing

public class ui_advancementwidget : MonoBehaviour
{
    public ui_list list;

    public void RenderAchievements()
    {
        list.ClearAllListElements();

        for (int i=0; i < Settings.advancementData.Count; i++)
        {
            // instead of sending the data through the list system, we're doing in manually
            // cuz its easier
            GameObject g_new = list.AddItem("");

            ui_advancementelement comp = g_new.GetComponent<ui_advancementelement>();

            comp.tx_name.text = Settings.advancementData[i].name;
            comp.tx_description.text = Settings.advancementData[i].description;
            comp.icon.texture = Settings.advancementData[i].icon;

            // checking if the player has an achievement, and changing the background color
            if (Settings.DoesPlayerHaveAdvancement(Settings.advancementData[i].name))
            {
                comp.bg.color = Color.darkGreen;
            } else
            {
                comp.bg.color = Color.darkRed;
            }
        }
    }
}
