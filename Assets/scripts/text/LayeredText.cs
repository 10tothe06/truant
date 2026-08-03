using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LayeredText : MonoBehaviour
{
    public bool layering_enabled = true;
    public TextMeshProUGUI[] text_layers;
    private Vector3[] default_local_positions;


    // do we shift the layers around over time?
    public bool wiggle_layers = false;
    // do we leave the top layer alone for the sake of legibility? (YES)
    public bool freeze_top_layer = true;
    public float wiggle_frequency;
    public float wiggle_amplitude;

    public bool draw_on_awake;
    public string awake_message;

    [HideInInspector]
    public string current_message;

    #region FORMATTING


    // these three functions are a bit of a cv but that's fine

    public void SetBold(bool b)
    {
        for (int i = 0; i < text_layers.Length; i++)
        {
            if (b)
            {
                text_layers[i].fontStyle = util_text.AddStyle(text_layers[i].fontStyle, FontStyles.Bold);
            } else
            {
                text_layers[i].fontStyle = util_text.RemoveStyle(text_layers[i].fontStyle, FontStyles.Bold);
            }
        }
    }


    public void SetItalic(bool b)
    {
        for (int i = 0; i < text_layers.Length; i++)
        {
            if (b)
            {
                text_layers[i].fontStyle = util_text.AddStyle(text_layers[i].fontStyle, FontStyles.Italic);
            } else
            {
                text_layers[i].fontStyle = util_text.RemoveStyle(text_layers[i].fontStyle, FontStyles.Italic);
            }
        }
    }

    public void SetUnderline(bool b)
    {
        for (int i = 0; i < text_layers.Length; i++)
        {
            if (b)
            {
                text_layers[i].fontStyle = util_text.AddStyle(text_layers[i].fontStyle, FontStyles.Underline);
            } else
            {
                text_layers[i].fontStyle = util_text.RemoveStyle(text_layers[i].fontStyle, FontStyles.Underline);
            }
        }
    }

    #endregion

    void Awake()
    {
        if (draw_on_awake)
        {
            Draw(awake_message);
        }

        default_local_positions = new Vector3[text_layers.Length];
        for (int i = 0; i < text_layers.Length; i++)
        {
            default_local_positions[i] = text_layers[i].transform.localPosition;
        }
    }

    public void Draw(string msg)
    {
        current_message = msg;

        for (int i = 0; i < text_layers.Length; i++)
        {
            text_layers[i].text = msg;
        }
    }

    void Update()
    {
        

        for (int i = 0; i < text_layers.Length; i++)
        {
            if (wiggle_layers)
            {
                if (freeze_top_layer && i == 0) {continue;}

                // the wiggling will be circular
                Vector3 v = new Vector3(Mathf.Sin(Time.time * wiggle_frequency), Mathf.Cos(Time.time * wiggle_frequency), 0) * wiggle_amplitude;

                v *= (i % 2 == 0) ? 1 : -1;

                text_layers[i].transform.localPosition = default_local_positions[i] + v;
            }
            
            if (layering_enabled)
            {
                text_layers[i].gameObject.SetActive(true);
            } else
            {
                text_layers[i].gameObject.SetActive(i == 0);
            }
        }
    }
}
