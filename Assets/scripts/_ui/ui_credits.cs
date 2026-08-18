using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ui_credits : MonoBehaviour
{
    public GameObject[] credits;


    void Awake()
    {
        for (int i = 0; i < credits.Length; i++)
        {
            credits[i].SetActive(false);
        }
    }

    public void RollCredits()
    {
        StartCoroutine(Credits());
    }

    public void CloseCredits()
    {
        credits[credits.Length-1].SetActive(false);
        AudioManager.StopAllMusic();
        UIManager.SetFadePercent(0f);
    }

    public IEnumerator Credits()
    {
        AudioManager.PlayMusic(0, 0.5f);

        yield return new WaitForSeconds(2f);
        
        // show the truant logo
        UIManager.SetFadePercent(1f);
        
        for (int i = 0; i < credits.Length; i++)
        {
            credits[i].SetActive(true);

            if (i < credits.Length - 1) // final credits object STAYS active
            {
                yield return new WaitForSeconds(2f);

                credits[i].SetActive(false);
            }
        }

    }
}
