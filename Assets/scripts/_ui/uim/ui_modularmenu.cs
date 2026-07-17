using UnityEngine;

// how exactly that variable gets displayed
// I might in the future add some sort of "possible variables" check (you can't display booleans with a slider)
// but for now it'll just be common sense
public enum uim_displaytype
{
    Slider,
    Checkbox,
    InputField,
    ColorPicker, // TODO:
    DropDown,
    Text,
}

// acts like a list, but with multiple types of elements
// sort of a composite-list
// used right now for game settings

public class uim_modularmenu : MonoBehaviour
{
    public Transform t_entryContainer;
    public float spaceBetweenItems;


    public void DrawMenu(uim_modularmenuentry[] entries)
    {
        ClearAllEntries();

        float totalHeight = 0;

        for (int i = 0; i < entries.Length; i++)
        {
            GameObject g_newEntry = Instantiate(uim.GetPrefabForType(entries[i].displayType), t_entryContainer);
            g_newEntry.transform.localPosition = -Vector3.up * totalHeight;

            ui_instantiatable comp = g_newEntry.GetComponent<ui_instantiatable>();
            comp.SetData(entries[i].data, entries[i].displayInfo);
            comp.onDataUpdate = entries[i].onDataUpdate;

            totalHeight += comp.effectiveHeight + spaceBetweenItems;
        }
    }

    public void ClearAllEntries()
    {
        ui_canvasutils.DestroyChildren(t_entryContainer.gameObject);
    }
}
