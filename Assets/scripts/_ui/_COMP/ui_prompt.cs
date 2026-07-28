using TMPro;
using UnityEngine;

public class ui_prompt : MonoBehaviour
{
    private static ui_prompt _instance;

    public static ui_prompt Instance
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
        DisplayPrompt("");
    }

    public TextMeshProUGUI tx;

    public void DisplayPrompt(string prompt)
    {
        tx.text = prompt;
    }
}
