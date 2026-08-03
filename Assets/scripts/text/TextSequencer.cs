using System.Collections;
using System.Data.Common;
using UnityEngine;

// handles the drawing of text sequences
// as the name may suggest

public class TextSequencer : MonoBehaviour
{
    [SerializeField]
    private AnimatedText text_component;
    private TextSequence current_sequence;

    public bool isRendering;

    public void RenderSequence(TextSequence data)
    {
        current_sequence = data;

        StartCoroutine(Draw());
    }

    public void Clear()
    {
        text_component.Clear();
    }

    private IEnumerator Draw()
    {
        isRendering = true;
        bool should_auto_wait;

        // looping through every message 
        for (int i = 0; i < current_sequence.messages.Length; i++)
        {
            // just skip blanks
            if (current_sequence.messages[i].Length < 1) {continue;}


            // check for any commands in the message
            if (current_sequence.messages[i][0] == ':')
            {
                float parsed_time = 0;
                float.TryParse(current_sequence.messages[i].Substring(1), out parsed_time);

                // colon means wait
                yield return new WaitForSeconds(parsed_time);


                should_auto_wait = false;
            } else
            {
                // if no command then its a message
                // the message is passed off to an AnimatedText component to be drawn
                text_component.Draw(current_sequence.messages[i]);


                should_auto_wait = true;
            }


            if (should_auto_wait)
            {
                yield return new WaitForSeconds(current_sequence.default_message_interval);
            }
        }

        isRendering = false;
    }
}
