using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ui_levelintro : MonoBehaviour
{
    public TextSequencer sequencer; 

    public Image bg;

    void Awake()
    {
        bg.gameObject.SetActive(false);
        sequencer.Clear();
    }

    public void PlayIntro(TextSequence data)
    {
        StartCoroutine(Intro(data));
    }

    private IEnumerator Intro(TextSequence data)
    {
        // fade to black
        bg.gameObject.SetActive(true);

        sequencer.RenderSequence(data);

        yield return new WaitUntil(() => {return !sequencer.isRendering;});

        // before we fade everything out and start the level,
        // we want to fade out all the text EXCEPT what's been highlighted

        // this is to show the player more clearly what their objective is,
        // in case they didn't already get the message thanks to the highlight and underline

        //yield return new WaitForSeconds(1f);
        sequencer.text_component.FadeOutText(true);

        yield return new WaitForSeconds(4f);

        sequencer.Clear();

        // fade back
        bg.gameObject.SetActive(false);
    }
}
