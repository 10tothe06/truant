using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ui_notehud : MonoBehaviour
{
    private static ui_notehud _instance;

    public static ui_notehud Instance
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

    private string current_note_text;
    private GameObject note_object;
    public bool is_holding_note {get; private set;}

    public GameObject g_prompt;
    public GameObject g_textContainer;
    public TextMeshProUGUI tx;
    

    public void PassNoteText(GameObject note_object, string text)
    {
        current_note_text = text;
        this.note_object = note_object;
        tx.text = text;

        is_holding_note = true;

        g_prompt.SetActive(true);
    }

    public void StopHoldingNote()
    {
        g_prompt.SetActive(false);
        is_holding_note = false;

        current_note_text = "";
    }

    void Update()
    {
        if (is_holding_note)
        {
            // show the prompt that allows the player to read the note in plain text

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                g_textContainer.SetActive(!g_textContainer.activeSelf);
            }

            // (if the game object gets destroyed)
            if (note_object == null)
            {
                StopHoldingNote();
            }
        }
    }
}
