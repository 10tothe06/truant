using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayeredText : MonoBehaviour
{
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
        for (int i = 0; i < text_layers.Length; i++)
        {
            text_layers[i].text = msg;
        }
    }

    void Update()
    {
        if (wiggle_layers)
        {
            for (int i = 0; i < text_layers.Length; i++)
            {
                if (freeze_top_layer && i == 0) {continue;}

                // the wiggling will be circular
                Vector3 v = new Vector3(Mathf.Sin(Time.time * wiggle_frequency), Mathf.Cos(Time.time * wiggle_frequency), 0) * wiggle_amplitude;

                v *= (i % 2 == 0) ? 1 : -1;

                text_layers[i].transform.localPosition = default_local_positions[i] + v;
            }
        }
    }
}
