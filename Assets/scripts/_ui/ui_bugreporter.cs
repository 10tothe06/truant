using TMPro;
using UnityEngine;

public class ui_bugreporter : MonoBehaviour
{
    public TMP_InputField in_bugTitle;
    public TMP_InputField in_bugDescription;

    public void SendBugReport()
    {
        string name = in_bugTitle.text;
        string description = in_bugDescription.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
        {
            //ui_infoalerts.Instance.ShowFullscreenAlert("please actually supply information", Color.red);
        }

        // so the player has at least written SOMETHING, 
        // this is the part where we send it
        
        util_email.SendEmail(new System.Collections.Generic.List<string>(){"maximilianmcdiarmid@gmail.com"}, name, description);
    }
}
