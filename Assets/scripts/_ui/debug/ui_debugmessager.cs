using UnityEngine;

// script that handles the debug messages coming at you from the bottom left of your screen

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

    public GameObject p_message;

    public static void PostMessage(string msg) {
        GameObject g_newMessage = Instantiate(Instance.p_message, Instance.transform);

        
    }
}
