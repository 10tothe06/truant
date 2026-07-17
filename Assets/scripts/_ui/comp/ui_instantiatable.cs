using UnityEngine;
using UnityEngine.Events;

// literally any ui thing that can be cloned/used

public class ui_instantiatable : MonoBehaviour
{
    public string heldData;
    public bool dataFlag;
    public string displayInfo;

    public UnityEvent<string> onDataUpdate;


    public float effectiveHeight;

    public void SetData(string data)
    {
        heldData = data;
        dataFlag = true; // tells other scripts that there has been a data update
        onDataUpdate.Invoke(data);
    }
    public void SetData(string data, string displayInfo)
    {
        heldData = data;
        dataFlag = true; // tells other scripts that there has been a data update
        this.displayInfo = displayInfo;
        onDataUpdate.Invoke(data);
    }
}
