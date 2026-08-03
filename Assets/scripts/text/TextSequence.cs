using UnityEngine;

// used for character monologues,
// level intros,
// etc.


// im trying to make this as modular as possible for now

[System.Serializable]
public class TextSequence
{
    // how long to wait if no wait command is read
    // (wait commands OVERWRITE, they do not ADD to this)

    // if we dont want to wait,
    // just make this 0
    public float default_message_interval;


    public string[] messages;



    public TextSequence() {}


    public TextSequence(string[] messages, float default_message_interval = 1f)
    {
        this.messages = messages;
        this.default_message_interval = default_message_interval;
    }
}
