using TMPro;
using UnityEngine;

// ********************************************
// updated and approved as of:
// 07/18/2026
// ********************************************

public class ui_stringdisplay : MonoBehaviour
{
    public bool configure_automatically = true;
    public TextMeshProUGUI tx;

    void Awake()
    {
        if (configure_automatically)
        {
            ui_instantiatable comp = GetComponent<ui_instantiatable>();

            if (comp != null)
            {
                comp.onDataUpdate.AddListener(Display);
            }
        }
    }

    public void Display(ui_instantiatable comp)
    {
        Display(comp.heldData);
    }

    public void Display(string str)
    {
        tx.text = str;
    }

    public void SetColor(Color col)
    {
        tx.color = col;
    }
}
