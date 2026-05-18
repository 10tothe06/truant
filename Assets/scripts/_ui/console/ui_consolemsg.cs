using TMPro;
using UnityEngine;

// why am I adding another layer of abstraction to this?
// why not just set the text directly?


// no clue
// hopefully one day this class will become useful

public class ui_consolemsg : MonoBehaviour
{
    public TextMeshProUGUI tx;

    public void SetData(string data)
    {
        tx.text = data;
    }

    public void SetColor(Color col)
    {
        tx.color = col;
    }
}
