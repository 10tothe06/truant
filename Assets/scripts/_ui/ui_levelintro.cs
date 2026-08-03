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

        sequencer.Clear();

        // fade back
        bg.gameObject.SetActive(false);
    }
}
