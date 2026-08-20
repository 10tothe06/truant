using TMPro;
using UnityEngine;

public class ui_playerhud : MonoBehaviour
{
    private static ui_playerhud _instance;

    public static ui_playerhud Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }


    public Transform t_itemPromptContainer;

    [HideInInspector]
    public GameObject g_promptObject;

    private string[] current_prompts;


    public GameObject p_itemPrompt;
    public float prompt_spacing;

    public static void DrawItemPrompts(GameObject obj, string[] prompts)
    {
        if (obj == Instance.g_promptObject)
        {
            prompts = util_array.Combine(prompts, Instance.current_prompts);
        }

        Instance.g_promptObject = obj;
        Instance.current_prompts = prompts;

        // first, clear any existing prompts
        ClearItemPrompt();

        // then, for each prompt we received, make a prompt object and set the text
        for (int i = 0; i < prompts.Length; i++)
        {
            GameObject g_newPrompt = Instantiate(Instance.p_itemPrompt, Instance.t_itemPromptContainer);

            g_newPrompt.transform.localPosition = -Vector3.up * i * Instance.prompt_spacing;

            g_newPrompt.GetComponent<TextMeshProUGUI>().text = prompts[i];
        }
    }

    public static void ClearItemPrompt()
    {
        util_canvas.DestroyChildren(Instance.t_itemPromptContainer);
    }
}
