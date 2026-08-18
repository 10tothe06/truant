using System.Collections.Generic;
using TMPro;
using UnityEngine;

// script that handles the debug messages coming at you from the bottom left of your screen

// (basically just a FIFO queue)

public class ui_debugmessager : MonoBehaviour
{
    private static ui_debugmessager _instance;

    public static ui_debugmessager Instance
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
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }


    public List<float> message_queue;
    public float message_spacing;


    [Header("FADING")]
    // how long the message hangs out at full opacity
    public float message_hang_time;
    // how long it takes for each message to fade out,
    // once they start to fade
    public float message_fade_time;
    


    public GameObject p_message;

    public static void PostMessage(string msg) {
        GameObject g_newMessage = Instantiate(Instance.p_message, Instance.transform);
        g_newMessage.GetComponent<TextMeshProUGUI>().text = msg;

        // local position should be Vector3.zero
        // other messages will be moved up in Update()

        // add the time that the message was created to the top of the queue
        Instance.message_queue.Add(Time.time);
    }

    void Update()
    {
        for (int i = message_queue.Count - 1; i >= 0; i--)
        {
            // first we figure out if the message should be deleted
            if (Time.time > message_queue[i] + message_hang_time + message_fade_time)
            {
                Destroy(transform.GetChild(i).gameObject);
                message_queue.RemoveAt(i);

                break;
            }

            TextMeshProUGUI comp = transform.GetChild(i).GetComponent<TextMeshProUGUI>();

            // handle message positioning
            // (new messages are lower than old ones)
            comp.transform.position = Vector3.up * message_spacing * (message_queue.Count - 1 - i);

            // if not, then we control the opacity
            if (Time.time > message_queue[i] + message_hang_time)
            {
                float alpha = 1 - (Time.time - message_queue[i] - message_hang_time) / message_fade_time;
                Color c = comp.color;

                comp.color = new Color(c.r, c.g, c.b, alpha);
            } else
            {
                // full opacity
                Color c = comp.color;

                comp.color = new Color(c.r, c.g, c.b, 1f);
            }
        }
    }
}
