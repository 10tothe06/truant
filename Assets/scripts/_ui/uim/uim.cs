using UnityEngine;

// static storage for the different components, that's all

// does this project have too many scripts?

// anyways I can't do this on ui_modularmenu else I'd have to add the references every time
public class uim : MonoBehaviour // short name so that it can be referenced quickly
{
    private void Awake()
    {
        text = p_text;
        inputField = p_inputField;
        button = p_button;

        colorPicker = p_colorPicker;
        checkbox = p_checkbox;
        dropdown = p_dropdown;
        slider = p_slider;
    }

    // technically these are ALL PREFABS, I'm just only using the prefix on the inspector ones
    // the static ones don't have any prefix so they're less verbose
    public GameObject p_text;
    public static GameObject text;

    public GameObject p_inputField;
    public static GameObject inputField;

    public GameObject p_button;
    public static GameObject button;

    public GameObject p_colorPicker;
    public static GameObject colorPicker;

    public GameObject p_checkbox;
    public static GameObject checkbox;

    public GameObject p_dropdown;
    public static GameObject dropdown;

    public GameObject p_slider;
    public static GameObject slider;

    public static GameObject GetPrefabForType(ushort displayType)
    {
        if (displayType == (ushort)uim_displaytype.Checkbox)
        {
            return checkbox;
        } else if (displayType == (ushort)uim_displaytype.ColorPicker)
        {
            return colorPicker;
        } else if (displayType == (ushort)uim_displaytype.DropDown)
        {
            return dropdown;
        } else if (displayType == (ushort)uim_displaytype.InputField)
        {
            return inputField;
        } else if (displayType == (ushort)uim_displaytype.Slider)
        {
            return slider;
        } else if (displayType == (ushort)uim_displaytype.Text)
        {
            return text;
        } else
        {
            return null;
        }
    }
}
