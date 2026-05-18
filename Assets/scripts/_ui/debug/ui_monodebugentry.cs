using System;
using TMPro;
using UnityEngine;

public class ui_monodebugentry : MonoBehaviour
{
    public TextMeshProUGUI tx;
    public ui_debugentry data;

    public void Initialize(ui_debugentry data)
    {
        this.data = data;
    }

    public void UpdateData()
    {
        if (ui_debugmenu.Instance.IsTabActive(data.tab))
        {
            tx.gameObject.SetActive(true);
        } else {tx.gameObject.SetActive(false);}
        
        tx.text = data.title + ":  " + data.dataSource.Invoke();
    }
}
