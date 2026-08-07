using UnityEngine;

public class util_items
{
    public static bool IsItemAllowed(inv_itemdata item_data, string[] parameters)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] == item_data.item_name)
            {
                // we are letting that item specifically in
                return true;
            }
            if (item_data.item_tags.Contains(parameters[i]))
            {
                // we are letting the item in bc it has a certain tag
                return true;
            }
        }


        return false;
    }
}
