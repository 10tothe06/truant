using TMPro;
using UnityEngine;

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
    private float default_character_interval;
    private float current_character_interval;
    private float last_character_time;

    [Space(10)]
    [Header("PREFABS")]
    public GameObject p_letter;


    private string current_msg;
    private int current_character_index;
    private bool is_drawing;


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
        // adding the character
        LayeredText tx_new = Instantiate(p_letter, transform).GetComponent<LayeredText>();
        tx_new.Draw(current_msg[current_character_index].ToString());
        
        tx_new.transform.localPosition = Vector3.right * current_character_index * 50f;
        tx_new.GetComponent<AnimatedCharacter>().AnimateIn();

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
