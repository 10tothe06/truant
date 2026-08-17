using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ui_levelintro : MonoBehaviour
{
    public TextSequencer sequencer; 

    public Image bg;
    public TextMeshProUGUI skip_text;

    void Awake()
    {
        bg.gameObject.SetActive(false);
        sequencer.Clear();
    }


    void Update()
    {
        // by pressing left ctrl, 
        // the user can skip the entire intro sequence
        
        // this is for if they've already played the level
        // and don't really care what the intro has to say
        // (cuz they've seen it already)

        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame && skip_text.gameObject.activeSelf)
        {
            sequencer.ForceStop();
            EndLevelIntro();
        }
    }


    public void PlayIntro(TextSequence data)
    {
        skip_text.gameObject.SetActive(true);
        
        StartCoroutine(Intro(data));
    }

    private void EndLevelIntro()
    {
        sequencer.Clear();

        
        bg.gameObject.SetActive(false);
        skip_text.gameObject.SetActive(false);

        // fade back
        UIManager.SetFadePercent(1f);
        UIManager.FadeOut();
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

        EndLevelIntro();
    }
}
