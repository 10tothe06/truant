using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ui_clicktype
{
    Primary, // LMB usually
    Secondary, // RMB usually
    Either,
}

public class ui_button : MonoBehaviour
{
    public ui_clicktype clickType;

    public bool isClickable = true;
    public bool enablePassthrough = false;
    public bool logWhenClicked = false;
    private bool isPressed;

    [Space(6)]
    [Header("Interactions")]
    public UnityEvent onPress; // Click (runs once)
    public UnityEvent<ushort> onPressSpecific;
    public UnityEvent onDrag; // Click and then move mouse away (runs once)
    public UnityEvent<ushort> onDragSpecific;
    public UnityEvent whilePress; // Hold click (repeats)

    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent whileHover;
    private bool isHovering;

    [Space(6)]
    [Header("Color Settings")]
    public bool colorSwitch;
    public bool leaveAlpha = true; // don't change the alpha of the button (allows other scripts to control it)
    public Color defaultColor;
    public Color hoverColor;
    public Color pressedColor;

    private ushort cachedClickType;

    private Image i;

    void Awake()
    {
        i = GetComponent<Image>();
    }

    private bool CheckInteraction()
    {
        if (enablePassthrough)
        {
            return ui_canvasutils.IsCursorInBounds(gameObject, true);
        } else
        {
            return ui_canvasutils.IsCursorInteract(gameObject, true);
        }
    }

    private bool IsClicking()
    {
        if (clickType == ui_clicktype.Primary)
        {
            return Input.mouseButtonDownLeft;
        } else if (clickType == ui_clicktype.Secondary)
        {
            return Input.mouseButtonDownRight;
        } else if (clickType == ui_clicktype.Either)
        {
            return Input.mouseButtonDownRight || Input.mouseButtonDownLeft;
        }

        return false;
    }

    void Update() {
        if (Input.mouseButtonDownLeft)
        {
            cachedClickType = 0;
        } else if (Input.mouseButtonDownRight)
        {
            cachedClickType = 1;
        }


        //if (!isClickable) {return;}

        if (IsClicking() && CheckInteraction()) {
            isPressed = true;
        }

        if (isPressed) {
            whilePress.Invoke(); // Invoke the event that runs when the button is held

             if (colorSwitch) { SetColor(pressedColor); }
        }
        else if (CheckInteraction()) {
            if (colorSwitch) { SetColor(hoverColor); }
            whileHover.Invoke();
            if (!isHovering) {onHoverEnter.Invoke(); isHovering = true;}
        }
        else {
            if (colorSwitch) { SetColor(defaultColor); }
            if (isHovering) {onHoverExit.Invoke(); isHovering = false;}
        }   

        if (!IsClicking()) {
            if (isPressed && CheckInteraction()) { onPress.Invoke(); onPressSpecific.Invoke(cachedClickType); if (logWhenClicked){Debug.Log("Click!");}}
            isPressed = false;
        }

        if (isPressed && !CheckInteraction()) {
            onDrag.Invoke(); // Invoke the unity event that runs when you click and drag the button
            onDragSpecific.Invoke(cachedClickType);
            isPressed = false;
        }
    }

    public void SetColor(Color col)
    {
        if (leaveAlpha)
        {
            i.color = new Color(col.r, col.g, col.b, i.color.a);
        } else
        {
            i.color = col;
        }
    }

    public void TestButton() {
        Debug.Log("Test Succesful?");
    }
}