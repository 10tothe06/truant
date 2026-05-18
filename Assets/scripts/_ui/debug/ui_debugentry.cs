using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ui_debugentry
{
    public string tab; // what sub-category of entries is this associated with
    public string title;
    public Func<string> dataSource;

    public ui_debugentry() {}
    public ui_debugentry(string title, Func<string> dataSource)
    {
        this.title = title;
        this.dataSource = dataSource;
        this.tab = "main";
    }
    public ui_debugentry(string title, Func<string> dataSource, string tab)
    {
        this.title = title;
        this.dataSource = dataSource;
        this.tab = tab;
    }
}
