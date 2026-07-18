using UnityEngine;

public class ui_settingsmenu : MonoBehaviour
{
    public void EnterMenu()
    {
        GetComponent<uim_modularmenu>().DrawMenu(Settings.GetModularEntries());
    }
}
