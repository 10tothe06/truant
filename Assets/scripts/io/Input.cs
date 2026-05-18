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

    void Update()
    {
        UpdateValues(Time.deltaTime);
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