using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class uim_modularmenuentry
{
    public string data;
    public string displayInfo;
    public UnityEvent<string> onDataUpdate;
    public ushort displayType;
    
    public uim_modularmenuentry() {onDataUpdate = new UnityEvent<string>();}

    public uim_modularmenuentry(string data, string displayInfo, UnityEvent<string> onDataUpdate, ushort displayType)
    {
        
    }
}
