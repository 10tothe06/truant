using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ui_debugmenu : MonoBehaviour
{
    private static ui_debugmenu _instance;

    public static ui_debugmenu Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public List<ui_debugtab> tabs;
    public List<ui_debugentry> entries;
    public GameObject p_entry;

    public Transform t_entryContainer;
    public List<ui_monodebugentry> monoEntries;

    public float entrySpacing;

    public void SetTabActive(string tabName, bool active)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].name == tabName)
            {
                tabs[i].isActive = active;
            }
        }
    }

    public void AddEntry(string title, Func<string> dataSource)
    {
        AddEntry(new ui_debugentry(title, dataSource));
        UpdateListOfTabs();
    }
    public void AddEntry(string title, Func<string> dataSource, string tab)
    {
        AddEntry(new ui_debugentry(title, dataSource,tab));
        UpdateListOfTabs();
    }

    public void UpdateListOfTabs()
    {
        List<string> tabNames = new List<string>();
        for (int i = 0; i < entries.Count; i++)
        {
            if (!tabNames.Contains(entries[i].tab))
            {
                tabNames.Add(entries[i].tab);
            }
        }

        for (int i = 0; i < tabNames.Count; i++)
        {
            if (!HasTabWithName(tabNames[i]))
            {
                tabs.Add(new ui_debugtab(tabNames[i]));
            }
        }
    }

    public bool HasTabWithName(string name)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].name == name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsTabActive(string name)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].name == name && tabs[i].isActive)
            {
                return true;
            }
        }
        return false;
    }

    public void AddEntry(ui_debugentry entry)
    {
        if (HasEntryWithName(entry.title)) {return;}
        entries.Add(entry);
        SpawnEntryObject(entry);
    }

    public bool HasEntryWithName(string name)
    {
        for (int i = 0; i < entries.Count;i ++)
        {
            if (entries[i].title == name)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveEntryByName(string name)
    {
        for (int i = 0; i < entries.Count;i ++)
        {
            if (entries[i].title == name)
            {
                entries.RemoveAt(i);
                break; // otherwise index math goes wrong
            }
        }
    }

    void Update()
    {
        // temp?
        UpdateAllEntries();
    }

    public void UpdateAllEntries()
    {
        if (t_entryContainer.childCount != entries.Count)
        {
            RefreshEntryList();
        }

        for (int i = 0; i < entries.Count; i++)
        {
            monoEntries[i].UpdateData();
        }
    }

    public void RefreshEntryList()
    {
        monoEntries = new List<ui_monodebugentry>();
        ui_canvasutils.DestroyChildren(t_entryContainer.gameObject);

        for (int i = 0; i < entries.Count;i++)
        {
            SpawnEntryObject(entries[i],i);
        }
    }

    // assuming last in list
    public void SpawnEntryObject(ui_debugentry entry)
    {
        SpawnEntryObject(entry, entries.Count - 1);
    }
    public void SpawnEntryObject(ui_debugentry entry, int indexInList)
    {
        GameObject g_newEntry = Instantiate(p_entry, t_entryContainer);
        g_newEntry.name = entry.title;

        g_newEntry.transform.localPosition = -Vector3.up * indexInList * entrySpacing;

        ui_monodebugentry comp = g_newEntry.GetComponent<ui_monodebugentry>();
        monoEntries.Add(comp);
        comp.Initialize(entry);
    }
}
