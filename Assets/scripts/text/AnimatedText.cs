using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AnimatedText : MonoBehaviour
{
    [Header("EFFECTS")]
    public bool enable_slide;
    public bool enable_fade;


    [Space(10)]
    [Header("CONFIG")]

    
    public bool draw_on_awake;
    public string awake_message;

    // time between characters
    [SerializeField]
    private float text_spacing;
    [SerializeField]
    private float default_character_interval;
    private float current_character_interval;
    private float last_character_time;

    [Space(10)]
    [Header("PREFABS")]
    public GameObject p_letter;


    // tracking variables
    // ***
    
    private string current_msg;
    private int current_character_index;
    private int character_index_offset; // to account for formatting characters
    private bool is_drawing;

    // vvv formatting vvv
    private bool is_layered;
    private bool is_underline;
    private bool is_italic;
    private bool is_bold;

    // ***


    private LayeredText tx_main;

    void Awake()
    {
        if (draw_on_awake)
        {
            Draw(awake_message);
        }
    }

    private void Initialize()
    {
        // first, clear all children
        util_canvas.DestroyChildren(gameObject);

        // now we make the main text
        tx_main = Instantiate(p_letter, transform).GetComponent<LayeredText>();
        tx_main.Draw("");

        // resetting the formatting trackers
        is_layered = false;
        is_underline = false;
        is_italic = false;
        is_bold = false;
    }


    // TODO: formatting for speed and such
    public void Draw(string msg)
    {
        Initialize();

        current_msg = msg;
        current_character_index = 0;

        last_character_time = 0;
        current_character_interval = default_character_interval;

        is_drawing = true;
    }

    private void RenderNextCharacter()
    {
        char to_render = current_msg[current_character_index];

        if (to_render == '$')
        {
            is_layered = !is_layered;
            character_index_offset ++;
        } else if (to_render == '|')
        {
            is_underline = !is_underline;
            character_index_offset ++;
        } else if (to_render == '*')
        {
            is_italic = !is_italic;
            character_index_offset ++;
        }else if (to_render == '#')
        {
            is_bold = !is_bold;
            character_index_offset ++;
        }else
        {
            // adding the character
            LayeredText tx_new = Instantiate(p_letter, transform).GetComponent<LayeredText>();

            // dealing with formatting
            tx_new.layering_enabled = is_layered;
            tx_new.SetBold(is_bold);
            tx_new.SetItalic(is_italic);
            tx_new.SetUnderline(is_underline);

            tx_new.Draw(current_msg[current_character_index].ToString());
            
            tx_new.transform.localPosition = Vector3.right * (current_character_index-character_index_offset) * text_spacing;
            tx_new.GetComponent<AnimatedCharacter>().AnimateIn(enable_slide, enable_fade);
        }

        // prepping for the next character
        current_character_index++;
        last_character_time = Time.time;

        if (current_character_index >= current_msg.Length)
        {
            StopDrawing();
        } else
        {
            current_character_interval = default_character_interval;
        }
    }

    private void StopDrawing()
    {
        is_drawing = false;        
    }

    void Update()
    {
        if (is_drawing)
        {
            if (Time.time > last_character_time + current_character_interval)
            {
                RenderNextCharacter();
            }
        }
    }
}
