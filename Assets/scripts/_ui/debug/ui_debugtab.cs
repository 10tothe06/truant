using UnityEngine;

[System.Serializable]
public class ui_debugtab
{
    public bool isActive;
    public string name;

    public ui_debugtab() {}
    public ui_debugtab(string name)
    {
        this.name = name;
        isActive = true;
    }
    public ui_debugtab(bool isActive, string name)
    {
        this.name = name;
        this.isActive = isActive;
    }
}
