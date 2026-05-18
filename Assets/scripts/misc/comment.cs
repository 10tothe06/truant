using UnityEngine;

// new idea
// writing comments in the inspector using this mono,
// so that weird game objects can be explained to future me

public class comment : MonoBehaviour
{
    [TextArea(1, 50)]
    public string text;
}
