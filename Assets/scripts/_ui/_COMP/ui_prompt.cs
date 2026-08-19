using TMPro;
using UnityEngine;

public class ui_prompt : MonoBehaviour
{

    void Awake()
    {
        DisplayPrompt("");
    }

    public TextMeshProUGUI tx;

    public void DisplayPrompt(string prompt)
    {
        tx.text = prompt;
    }
}
