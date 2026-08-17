using UnityEngine;
using UnityEngine.InputSystem;

// re-creating the old unity input system, for ease of use

public class Input : MonoBehaviour
{
    private static Input _instance;

    public static Input Instance
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
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    [Header("CONFIG")]
    public float axisSpringAmount;

    // the axes, like from the old input system ****
    public static float inputAxisHorizontal;
    public static float inputAxisForward;
    public static float inputAxisVertical;
    // ****

    public static float scrollWheelAxis;

    public static Vector2 mouseMovement;
    public static bool mouseButtonLeft;
    public static bool mouseButtonDownLeft;
    public static bool mouseButtonRight;
    public static bool mouseButtonDownRight;

    public static Vector2 mousePosition;


    private bool isTyping;
    private float typing_timer_start;
    private float typing_timer_duration = 0.5f;

    

    void Update()
    {
        UpdateValues(Time.deltaTime);

        // the console/cheats menu
        if (Keyboard.current.minusKey.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            // console
            UIManager.ToggleConsole();
        }
        if (Keyboard.current.equalsKey.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            // cheats
            UIManager.ToggleCheatMenu();
        }

        // keyboard presses
        // TEMP until i can come up with a better input system
        if (!UIManager.isTyping)
        {

            // opening/closing the player inventory
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                
                UIManager.Instance.ToggleInventory();
            }

            // turning on and off the debug menu
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                UIManager.Instance.ToggleDebugMenu();
            }




            // selecting hotbar cells
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(0);
            } else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(1);
            } else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(2);
            } else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(3);
            } else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(4);
            } else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(5);
            } else if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(6);
            } else if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                Player.player_hotbar.SelectCell(7);
            }

            if (scrollWheelAxis > 0)
            {
                Player.player_hotbar.SelectNextCell(1);
            } else if (scrollWheelAxis < 0)
            {
                Player.player_hotbar.SelectNextCell(-1);
            }

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                Player.DropSelectedItem();
            }
        }



        if (Cursor.lockState != CursorLockMode.Locked)
        {
            isTyping = true;
        } else
        {
            // small delay to insure no interactions immediately after quitting menus
            if (typing_timer_start == -1)
            {
                typing_timer_start = Time.time;
            }
            else
            {
                if (Time.time > typing_timer_start + typing_timer_duration)
                {
                    typing_timer_start = -1;
                    isTyping = false;
                }
            }
        }
    }

    public static bool GetMouseButtonDown(int button)
    {
        if (button == 0) // left-click
        {
            return mouseButtonDownLeft;
        } else if (button == 1) // right-click
        {
            return mouseButtonDownRight;
        } else
        {
            return false;
        }
    }

    public static bool GetMouseButton(int button)
    {
        if (button == 0) // left-click
        {
            return mouseButtonLeft;
        } else if (button == 1) // right-click
        {
            return mouseButtonRight;
        } else
        {
            return false;
        }
    }

    // grabs which keys the player is pressing and turns them into this nice, clean, standard format
    public static player_keypresspacket GetKeypressPacket()
    {
        player_keypresspacket result = new player_keypresspacket();

        if (!UIManager.isTyping)
        {
            result.forward = Keyboard.current.wKey.isPressed;
            result.left = Keyboard.current.aKey.isPressed;
            result.back = Keyboard.current.sKey.isPressed;
            result.right = Keyboard.current.dKey.isPressed;

            result.jump = Keyboard.current.spaceKey.isPressed;

            result.sprint = Keyboard.current.shiftKey.isPressed;
            result.crouch = Keyboard.current.leftCtrlKey.isPressed;

            result.horizontalMouse = Input.mouseMovement.x;
            result.verticalMouse = Input.mouseMovement.y;

            result.up = Keyboard.current.eKey.isPressed;
            result.down = Keyboard.current.qKey.isPressed;

            result.mouseLeft = Input.mouseButtonLeft;
            result.mouseRight = Input.mouseButtonRight;

            result.isTyping = Instance.isTyping;
        }

        return result;
    }

    public void UpdateValues(float dt)
    { 
        mousePosition = Mouse.current.position.ReadValue();
        mouseMovement = Mouse.current.delta.ReadValue();


        if (Mouse.current.leftButton.ReadValue() > 0)
        {
            if (!mouseButtonLeft)
            {
                mouseButtonDownLeft = true;
            } else
            {
                mouseButtonDownLeft = false;
            }
            mouseButtonLeft = true;
        } else
        {
            
            mouseButtonLeft = false;
            mouseButtonDownLeft = false;
        }
        
        if (Mouse.current.rightButton.ReadValue() > 0)
        {
            if (!mouseButtonRight)
            {
                mouseButtonDownRight = true;
            } else
            {
                mouseButtonDownRight = false;
            }
            mouseButtonRight = true;
        } else
        {
            mouseButtonRight = false;
            mouseButtonDownRight = false;
        }
        

        scrollWheelAxis = Mouse.current.scroll.ReadValue().y;

        // WARNING: possible the t params for these lerps go above 1
        
        inputAxisHorizontal = Mathf.Lerp(inputAxisHorizontal, 0, axisSpringAmount * dt); // not entirely sure about the multiplying by dt
        inputAxisForward = Mathf.Lerp(inputAxisForward, 0, axisSpringAmount * dt);
        inputAxisVertical = Mathf.Lerp(inputAxisVertical, 0, axisSpringAmount * dt);

        if (Keyboard.current.qKey.isPressed)
        {
            inputAxisVertical = -1;
        }
        if (Keyboard.current.eKey.isPressed)
        {
            inputAxisVertical = 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            inputAxisForward = -1;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            inputAxisForward = 1;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            inputAxisHorizontal = -1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            inputAxisHorizontal = 1;
        }
    }
}