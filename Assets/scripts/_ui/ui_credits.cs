using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ui_credits : MonoBehaviour
{
    public Image i_gameLogo;


    void Awake()
    {
        i_gameLogo.gameObject.SetActive(false);
    }

    public void RollCredits()
    {
        StartCoroutine(Credits());
    }

    public IEnumerator Credits()
    {
        AudioManager.PlayMusic(0);

        yield return new WaitForSeconds(1f);

        i_gameLogo.gameObject.SetActive(true);
    }
}
