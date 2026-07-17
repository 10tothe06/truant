using UnityEngine;
using UnityEngine.InputSystem;

public class ui_navigator : MonoBehaviour
{
    private static ui_navigator _instance;

    public static ui_navigator Instance
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

    private ui_navigable current_nav;

    public void Set(ui_navigable nav)
    {
        current_nav = nav;
    }

    public void MoveTo(ui_navigable nav)
    {
        if (nav == null) {return;}


        current_nav.Deselect();

        nav.Select();

        Set(nav);
    }

    public void Clear()
    {
        if (current_nav != null)
        {
            current_nav.Deselect();
            current_nav = null;
        }
    }

    void Update()
    {
        if (current_nav != null)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                MoveTo(current_nav.n_up);
            } else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                MoveTo(current_nav.n_down);
            } else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                MoveTo(current_nav.n_left);
            } else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                MoveTo(current_nav.n_right);
            }
        }
    }
}
