using System.Collections.Generic;
using TMPro;
using UnityEngine;

// commands, etc.
// in-game chat will be handled differently, unlike minecraft where they are the same window

public class ui_console : MonoBehaviour
{
    private static ui_console _instance;
    public static ui_console Instance
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

    public TMP_InputField consoleInput;
    
    public GameObject p_msg;
    public Transform t_messageContainer;

    // for now, just a simple spacing system will do
    public float messageSpacing;

    public void PostMessage(string msg)
    {
        PostMessage(msg, Color.white);
    }
    public void PostMessage(string msg, Color col)
    {
        // first, move all existing messages up
        for (int i = 0; i < t_messageContainer.childCount; i++)
        {
            t_messageContainer.GetChild(i).position += Vector3.up * messageSpacing;
        }

        ui_consolemsg newMsg = Instantiate(p_msg, t_messageContainer).GetComponent<ui_consolemsg>();
        newMsg.transform.localPosition = Vector3.zero;
        newMsg.SetColor(col);

        newMsg.SetData(msg);
    }
}
